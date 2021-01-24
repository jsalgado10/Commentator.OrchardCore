using Notifications.OrchardCore.Services;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.DisplayManagement;
using OrchardCore.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Commentator.OrchardCore.Services
{
    public class CommentatorNotificationService : BaseNotificationService
    {
        public override Task<IList<MailMessage>> GetNotificationsPublishedAsync(PublishContentContext context)
        {
            return base.GetNotificationsPublishedAsync(context);
        }
    }
}
