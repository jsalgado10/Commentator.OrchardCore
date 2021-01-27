using System;
using System.Collections.Generic;
using System.Text;

namespace Commentator.OrchardCore.Settings
{
    public class CommentatorNotificationsSettings
    {
        public bool SendCommentNotifications { get; set; }
        public string CommentReplySubjectMessage { get; set; }
        public string CommentReplyEmailMessage { get; set; }
        public string CommentMentionSubjectMessage { get; set; }
        public string CommentMentionEmailMessage { get; set; }
    }
}
