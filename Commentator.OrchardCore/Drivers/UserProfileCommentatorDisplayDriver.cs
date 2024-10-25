using System;
using System.Threading.Tasks;
using Commentator.OrchardCore.Models;
using Commentator.OrchardCore.ViewModels;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Users.Models;

namespace Commentator.OrchardCore.Drivers
{
    public class UserProfileCommentatorDisplayDriver : SectionDisplayDriver<User, UserProfileCommentator>
    {
        public override Task<IDisplayResult> EditAsync(User user, UserProfileCommentator profile, BuildEditorContext context)
        {
            var result = Initialize<EditUserProfileCommentatorViewModel>("UserProfileCommentator_Edit", model =>
            {
                model.NotificationCommentOnReplies = profile.NotificationCommentOnReplies;
                model.NotificationCommentOnMentions = profile.NotificationCommentOnMentions;

            }).Location("Content:2#Notifications");

            return Task.FromResult<IDisplayResult>(result);
        }

        public override async Task<IDisplayResult> UpdateAsync(User user, UserProfileCommentator profile, UpdateEditorContext context)
        {
            var model = new EditUserProfileCommentatorViewModel();

            if (await context.Updater.TryUpdateModelAsync(model, Prefix))
            {
                profile.NotificationCommentOnReplies = model.NotificationCommentOnReplies;
                profile.NotificationCommentOnMentions = model.NotificationCommentOnMentions;
            }

            return await EditAsync(user, profile, context);
        }
    }
}
