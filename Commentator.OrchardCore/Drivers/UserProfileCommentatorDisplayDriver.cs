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
        public override IDisplayResult Edit(UserProfileCommentator profile, BuildEditorContext context)
        {
            return Initialize<EditUserProfileCommentatorViewModel>("UserProfileCommentator_Edit", model =>
            {
                model.NotificationCommentOnReplies = profile.NotificationCommentOnReplies;
                model.NotificationCommentOnMentions = profile.NotificationCommentOnMentions;

            }).Location("Content:2");
        }

        public override async Task<IDisplayResult> UpdateAsync(UserProfileCommentator profile, BuildEditorContext context)
        {
            var model = new EditUserProfileCommentatorViewModel();

            if (await context.Updater.TryUpdateModelAsync(model, Prefix))
            {
                profile.NotificationCommentOnReplies = model.NotificationCommentOnReplies;
                profile.NotificationCommentOnMentions = model.NotificationCommentOnMentions;
            }

            return Edit(profile, context);
        }
    }
}
