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
    public class EmailNotificationService : INotificationService
    {
        private readonly ISmtpService emailService;
        private readonly dynamic New;

        public EmailNotificationService(IShapeFactory shapeFactory, ISmtpService currentEmailService)
        {
            New = shapeFactory;
            emailService = currentEmailService;
        }
        public Task SendAsync(ContentContextBase context)
        {
            MailMessage message = new MailMessage()
            {
                To = "jsalgado@dcrpos.com",
                From = "notification@dcrpos.com",
                Subject = "Test email",
                Body = "Notification on Commentator Module"
            };

            return emailService.SendAsync(message);
        }
    }
}
