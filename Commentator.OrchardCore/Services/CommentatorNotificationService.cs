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
            var notificationSettings = siteSettings.GetSiteSettingsAsync()
                .GetAwaiter().GetResult()
                .As<CommentatorNotificationsSettings>();
            var users = await session.Query<User, UserIndex>().ListAsync();
            MailMessage message;
            NotificationsContentViewModel model;
            List<MailMessage> messages = new List<MailMessage>();

            try
            {
                var document = await contentItemDisplayManager.BuildDisplayAsync(context.ContentItem, updateModelAccessor.ModelUpdater, "Summary");
                var isCommentReply = context.ContentItem.Content.CommentPost.CommentParent.Text.Value != "0";
                var commentContent = context.ContentItem.Content.CommentPost.CommentText.Text.Value;
                var mentionPattern = @"<span class=""mention"" data-mention=""@(\w+)"">";
                var mentions = Regex.Matches(commentContent, mentionPattern);
                foreach (Match mention in mentions)
                {
                    logger.LogInformation($@"Mentioned : {mention.Groups[1].Value}");
                }

                if (notificationSettings.SendCommentNotifications)
                {

                }
            }
            catch (Exception ex)
            {
                logger.LogError($@"GetNotificationsPublishedAsync Error: {ex.Message}");
            }

            return await Task.FromResult(messages);
        }
    }
}
