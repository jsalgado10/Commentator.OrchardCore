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
        //public override async Task<IList<MailMessage>> GetNotificationsPublishedAsync(PublishContentContext context)
        //{
        //    List<MailMessage> messages = new List<MailMessage>();

        //    if (context.ContentItem.ContentType == "CommentPost")
        //    {
        //        MailMessage message = new MailMessage()
        //        {
        //            To = "jsalgado@dcrpos.com",
        //            From = "notification@dcrpos.com",
        //            Subject = "Test email",
        //            Body = "Notification on Commentator"
        //        };
        //        messages.Add(message);
        //    }

        //    return await Task.FromResult(messages);
        //}
    }
}
