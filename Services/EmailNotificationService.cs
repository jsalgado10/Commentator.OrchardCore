using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement;
using OrchardCore.Email;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrchardCore.Commentator.Services
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
        public Task SendNotification(ContentItem item)
        {
            MailMessage message = new MailMessage()
            {
                To = "email@email.com",
                From = "notification@email.com",
                Subject = "Test email",
                Body = "Notification on Commentator Module"
            };

            return emailService.SendAsync(message);
        }
    }
}
