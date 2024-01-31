using Commentator.OrchardCore.Settings;
using Commentator.OrchardCore.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notifications.OrchardCore.Services;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Email;
using OrchardCore.Entities;
using OrchardCore.Settings;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using YesSql;
using YesSql.Services;
using System.IO;

namespace Commentator.OrchardCore.Services
{
    public class CommentatorNotificationService : BaseNotificationService
    {
        private readonly IDisplayHelper displayHelper;
        private readonly HtmlEncoder htmlEncoder;
        private readonly ISiteService siteSettings;
        private readonly ILogger logger;
        private readonly ISession session;
        private readonly IServiceProvider serviceProvider;
        private readonly IUpdateModelAccessor updateModelAccessor;

        public CommentatorNotificationService(
            IDisplayHelper currentDisplayHelper,
            HtmlEncoder currentHtmlDecoder,
            ISiteService currentSiteSettings,
            ILogger<CommentatorNotificationService> currentLogger,
            ISession currentSession,
            IServiceProvider currentServiceProvider,
            IUpdateModelAccessor currentUpdateModelAccessor
            )
        {
            displayHelper = currentDisplayHelper;
            htmlEncoder = currentHtmlDecoder;
            siteSettings = currentSiteSettings;
            logger = currentLogger;
            session = currentSession;
            serviceProvider = currentServiceProvider;
            updateModelAccessor = currentUpdateModelAccessor;
        }

        public override async Task<IList<MailMessage>> GetNotificationsPublishedAsync(PublishContentContext context)
        {
            var contentItemDisplayManager = serviceProvider.GetRequiredService<IContentItemDisplayManager>();
            var contentManager = serviceProvider.GetRequiredService<IContentManager>();
            var notificationSettings = siteSettings.GetSiteSettingsAsync()
                .GetAwaiter().GetResult()
                .As<CommentatorNotificationsSettings>();
            var users = await session.Query<User, UserIndex>().Where(user => user.IsEnabled).ListAsync();
            MailMessage message;
            CommentNotificationsContentViewModel model;
            List<MailMessage> messages = new List<MailMessage>();
            var document = context.ContentItem;

            if (document.ContentType == "CommentPost")
            {
                try
                {
                    if (notificationSettings.SendCommentNotifications)
                    {
                        var commentArticleId = context.ContentItem.Content.CommentPost.CommentArticle.Text.Value;
                        var commentArticle = await contentManager.GetAsync(commentArticleId);
                        var commentArticleShape = await contentItemDisplayManager.BuildDisplayAsync(commentArticle, updateModelAccessor.ModelUpdater, "Summary");
                        var commentArticleData = await BuildShapeOutput(new CommentNotificationsContentViewModel
                        {
                            TemplateName = "CommentArticle",
                            RecordItem = commentArticleShape
                        });
                        var isCommentReply = context.ContentItem.Content.CommentPost.CommentParent.Text.Value != "0";
                        var commentContent = context.ContentItem.Content.CommentPost.CommentText.Text.Value;
                        var mentionPattern = @"<span class=""mention"" data-mention=""@(\w+)"">";
                        var mentions = Regex.Matches(commentContent, mentionPattern);
                        List<string> mentionedUsernames = new List<string>();
                        foreach (Match mention in mentions)
                        {
                            mentionedUsernames.Add(mention.Groups[1].Value);
                            logger.LogInformation($@"Mentioned : {mention.Groups[1].Value}");
                        }

                        if (mentionedUsernames.Count > 0)
                        {
                            logger.LogInformation("Getting List of Users that want notifications on comment mentions");
                            var mentionedUsers = mentionedUsernames.Contains("All") ? users.Where(user => NotifyOnMentions(user)) : users.Where(user => mentionedUsernames.Contains(user.UserName) && NotifyOnMentions(user));
                            var mentionedEmailSubject = string.IsNullOrEmpty(notificationSettings.CommentMentionSubjectMessage) ? "You were mentioned in a comment" : notificationSettings.CommentMentionSubjectMessage;
                            var mentionedEmailMessage = string.IsNullOrEmpty(notificationSettings.CommentMentionEmailMessage) ? "Your name came up in the following comment" : notificationSettings.CommentMentionEmailMessage;
                            var mentionedCommentData = await BuildShapeOutput(new CommentNotificationsContentViewModel
                            {
                                TemplateName = "CommentMentioned",
                                RecordItem = document
                            });

                            foreach (var user in mentionedUsers)
                            {
                                model = new CommentNotificationsContentViewModel()
                                {
                                    TemplateName = "CommentatorBaseNotification",
                                    Message = mentionedEmailMessage,
                                    User = user.UserName,
                                    ContentData = mentionedCommentData,
                                    CommentArticleLink = commentArticleData
                                };

                                message = new MailMessage()
                                {
                                    To = user.Email,
                                    Subject = mentionedEmailSubject,
                                    Body = await BuildShapeOutput(model),
                                    IsHtmlBody = true
                                };

                                messages.Add(message);
                            }
                        }

                        if (isCommentReply)
                        {
                            logger.LogInformation("Getting List of Users that want notifications on comment replies");
                            var parentCommentId = context.ContentItem.Content.CommentPost.CommentParent.Text.Value;
                            dynamic parentComment = await contentManager.GetAsync(parentCommentId);
                            if (parentComment != null)
                            {
                                var parentCommentOwner = parentComment.Owner;
                                var replyUsers = users.Where(user => user.UserName == parentCommentOwner && NotifyOnReply(user));
                                var replyEmailSubject = string.IsNullOrEmpty(notificationSettings.CommentReplySubjectMessage) ? "A reply to your comment" : notificationSettings.CommentReplySubjectMessage;
                                var replyEmailMessage = string.IsNullOrEmpty(notificationSettings.CommentReplyEmailMessage) ? "Somebody reply to your comment" : notificationSettings.CommentReplyEmailMessage;
                                var replyCommentData = await BuildShapeOutput(new CommentNotificationsContentViewModel
                                {
                                    TemplateName = "CommentReply",
                                    RecordItem = document,
                                    CommentParentData = parentComment.Content.CommentPost.CommentText.Text
                                });

                                foreach (var user in replyUsers)
                                {
                                    model = new CommentNotificationsContentViewModel()
                                    {
                                        TemplateName = "CommentatorBaseNotification",
                                        Message = replyEmailMessage,
                                        User = user.UserName,
                                        ContentData = replyCommentData,
                                        CommentArticleLink = commentArticleData
                                    };

                                    message = new MailMessage()
                                    {
                                        To = user.Email,
                                        Subject = replyEmailSubject,
                                        Body = await BuildShapeOutput(model),
                                        IsHtmlBody = true
                                    };

                                    messages.Add(message);
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.LogInformation($@"Comment Notifications are disabled");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($@"GetNotificationsPublishedAsync Error: {ex.Message}");
                }
            }

            return await Task.FromResult(messages);
        }

        private bool NotifyOnMentions(dynamic user)
        {
            var result = (user.Properties?.UserProfileCommentator?.NotificationCommentOnMentions) ?? false;
            return result;
        }

        private bool NotifyOnReply(dynamic user)
        {
            var result = (user.Properties?.UserProfileCommentator?.NotificationCommentOnReplies) ?? false;
            return result;
        }

        private async Task<string> BuildShapeOutput(object model)
        {
            logger.LogInformation("Building Email Body");
            var body = string.Empty;
            using (var sw = new StringWriter())
            {
                var htmlContent = await displayHelper.ShapeExecuteAsync((IShape)model);
                htmlContent.WriteTo(sw, htmlEncoder);
                body = sw.ToString();
                logger.LogInformation($@"HTML Body Content : {body}");
            }

            return body;
        }
    }
}
