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
using Notifications.OrchardCore.Models;
using OrchardCore.Notifications;

namespace Commentator.OrchardCore.Services
{
    public class CommentatorNotificationService : BaseContentNotificationService
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

        public override async Task<IList<ContentNotification>> GetNotificationsPublishedAsync(PublishContentContext context)
        {
            var contentItemDisplayManager = serviceProvider.GetRequiredService<IContentItemDisplayManager>();
            var contentManager = serviceProvider.GetRequiredService<IContentManager>();
            var notificationSettings = siteSettings.GetSiteSettingsAsync()
                .GetAwaiter().GetResult()
                .As<CommentatorNotificationsSettings>();
            var users = await session.Query<User, UserIndex>().Where(user => user.IsEnabled).ListAsync();

            CommentNotificationsContentViewModel model;
            List<ContentNotification> notifications = new List<ContentNotification>();
            var document = context.ContentItem;

            if (document.ContentType == "CommentPost")
            {
                try
                {
                    if (notificationSettings.SendCommentNotifications)
                    {
                        string commentArticleId = context.ContentItem.Content.CommentPost.CommentArticle.Text;
                        var commentArticle = await contentManager.GetAsync(commentArticleId, VersionOptions.Latest);
                        var commentArticleShape = await contentItemDisplayManager.BuildDisplayAsync(commentArticle, updateModelAccessor.ModelUpdater, "Summary");
                        var commentArticleData = await BuildShapeOutput(new CommentNotificationsContentViewModel
                        {
                            TemplateName = "CommentArticle",
                            RecordItem = commentArticleShape
                        });
                        var isCommentReply = context.ContentItem.Content.CommentPost.CommentParent.Text != "0";
                        string commentContent = context.ContentItem.Content.CommentPost.CommentText.Text;
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
                            var mentionedUsers = mentionedUsernames.Contains("All") ? users.Where(user => NotifyOn(user, "NotificationCommentOnMentions")) : users.Where(user => mentionedUsernames.Contains(user.UserName) && NotifyOn(user, "NotificationCommentOnMentions"));
                            var mentionedEmailSubject = string.IsNullOrEmpty(notificationSettings.CommentMentionSubjectMessage) ? "You were mentioned in a comment" : notificationSettings.CommentMentionSubjectMessage;
                            var mentionedEmailMessage = string.IsNullOrEmpty(notificationSettings.CommentMentionEmailMessage) ? "Your name came up in the following comment" : notificationSettings.CommentMentionEmailMessage;
                            var mentionedCommentData = await BuildShapeOutput(new CommentNotificationsContentViewModel
                            {
                                TemplateName = "CommentMentioned",
                                RecordItem = document
                            });

                            foreach (var user in mentionedUsers)
                            {
                                ContentNotification notification;
                                INotificationMessage notificationMessage;
                                model = new CommentNotificationsContentViewModel()
                                {
                                    TemplateName = "CommentatorBaseNotification",
                                    Message = mentionedEmailMessage,
                                    User = user.UserName,
                                    ContentData = mentionedCommentData,
                                    CommentArticleLink = commentArticleData
                                };

                                notificationMessage = new ContentNotificationMessage()
                                {
                                    Subject = mentionedEmailSubject,
                                    Summary = "You were mentioned on a comment",
                                    TextBody = "You were mentioned on a comment",
                                    HtmlBody = await BuildShapeOutput(model),
                                    IsHtmlPreferred = true
                                };

                                notification = new ContentNotification()
                                {
                                    User = user,
                                    Message = notificationMessage
                                };

                                notifications.Add(notification);
                            }
                        }

                        if (isCommentReply)
                        {
                            logger.LogInformation("Getting List of Users that want notifications on comment replies");
                            string parentCommentId = context.ContentItem.Content.CommentPost.CommentParent.Text;
                            dynamic parentComment = await contentManager.GetAsync(parentCommentId,VersionOptions.Latest);
                            if (parentComment != null)
                            {
                                var parentCommentOwner = parentComment.Owner;
                                var replyUsers = users.Where(user => user.UserName == parentCommentOwner && NotifyOn(user, "NotificationCommentOnReplies"));
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
                                    ContentNotification notification;
                                    INotificationMessage notificationMessage;

                                    model = new CommentNotificationsContentViewModel()
                                    {
                                        TemplateName = "CommentatorBaseNotification",
                                        Message = replyEmailMessage,
                                        User = user.UserName,
                                        ContentData = replyCommentData,
                                        CommentArticleLink = commentArticleData
                                    };

                                    notificationMessage = new ContentNotificationMessage()
                                    {
                                        Subject = replyEmailSubject,
                                        Summary = "Someone replied to your comment",
                                        TextBody = "Someone replied to your comment",
                                        HtmlBody = await BuildShapeOutput(model),
                                        IsHtmlPreferred = true
                                    };

                                    notification = new ContentNotification()
                                    {
                                        User = user,
                                        Message = notificationMessage
                                    };

                                    notifications.Add(notification);
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

            return await Task.FromResult(notifications);
        }

        private bool NotifyOn(dynamic user, string property)
        {
            var result = user.Properties["UserProfileCommentator"][property].GetValue<bool>() ?? false;
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
