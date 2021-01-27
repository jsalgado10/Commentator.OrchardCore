using Commentator.OrchardCore.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commentator.OrchardCore.Drivers
{
    public class CommentatorNotificationsSettingsDisplayDriver : SectionDisplayDriver<ISite, CommentatorNotificationsSettings>
    {
        public const string GroupId = "notifications";
        private readonly IShellHost shellHost;
        private readonly ShellSettings shellSettings;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthorizationService authorizationService;
        private readonly IContentDefinitionManager contentDefinitionManager;

        public CommentatorNotificationsSettingsDisplayDriver(
            IContentDefinitionManager currentContentDefinitionManager,
            IShellHost currentShellHost,
            ShellSettings currentShellSettings,
            IHttpContextAccessor currentHttpContextAccessor,
            IAuthorizationService currentAuthorizationService)
        {
            shellHost = currentShellHost;
            shellSettings = currentShellSettings;
            httpContextAccessor = currentHttpContextAccessor;
            authorizationService = currentAuthorizationService;
            contentDefinitionManager = currentContentDefinitionManager;
        }

        public override async Task<IDisplayResult> EditAsync(CommentatorNotificationsSettings section, BuildEditorContext context)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (!await authorizationService.AuthorizeAsync(user, Permissions.ManageCommentNotificationsSettings))
            {
                return null;
            }

            var shapes = new List<IDisplayResult>
            {
                Initialize<CommentatorNotificationsSettings>("CommentsNotificationsSettings_Edit", model =>
                {
                    model.SendCommentNotifications = section.SendCommentNotifications;
                    model.CommentReplySubjectMessage = section.CommentReplySubjectMessage;
                    model.CommentReplyEmailMessage = section.CommentReplyEmailMessage;
                    model.CommentMentionSubjectMessage = section.CommentMentionSubjectMessage;
                    model.CommentMentionEmailMessage = section.CommentMentionEmailMessage;

                }).Location("Content:2#Comments").OnGroup(GroupId)
            };

            return Combine(shapes);
        }

        public override async Task<IDisplayResult> UpdateAsync(CommentatorNotificationsSettings section, BuildEditorContext context)
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (!await authorizationService.AuthorizeAsync(user, Permissions.ManageCommentNotificationsSettings))
            {
                return null;
            }

            if (context.GroupId == GroupId)
            {
                await context.Updater.TryUpdateModelAsync(section, Prefix);
                await shellHost.ReleaseShellContextAsync(shellSettings);
            }

            return await EditAsync(section, context);
        }
    }
}
