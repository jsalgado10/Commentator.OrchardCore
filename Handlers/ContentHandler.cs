using Commentator.OrchardCore.Services;
using OrchardCore.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Commentator.OrchardCore.Handlers
{
    public class ContentHandler : ContentHandlerBase
    {
        public readonly INotificationService notificationService;
        public ContentHandler(INotificationService currentNotificationService)
        {
            notificationService = currentNotificationService;
        }
        public override Task PublishedAsync(PublishContentContext context)
        {
            Console.WriteLine(context.ContentItem.ContentType.ToString());
            if (context.ContentItem.ContentType == "CommentPost")
            {
                return notificationService.SendNotification(context.ContentItem.ContentItem);
            }

            return Task.CompletedTask;
        }
    }
}
