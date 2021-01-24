//using Notifications.OrchardCore.Services;
//using OrchardCore.ContentManagement;
//using OrchardCore.ContentManagement.Handlers;
//using OrchardCore.DisplayManagement;
//using OrchardCore.Email;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Commentator.OrchardCore.Services
//{
//    public class CommentatorNotificationService : BaseNotificationService
//    {
//        public override async task<ilist<mailmessage>> getnotificationspublishedasync(publishcontentcontext context)
//        {
//            list<mailmessage> messages = new list<mailmessage>();

//            if (context.contentitem.contenttype == "commentpost")
//            {
//                mailmessage message = new mailmessage()
//                {
//                    to = "jsalgado@dcrpos.com",
//                    from = "notification@dcrpos.com",
//                    subject = "test email",
//                    body = "notification on commentator"
//                };
//                messages.add(message);
//            }

//            return await task.fromresult(messages);
//        }
//    }
//}
