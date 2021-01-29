using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commentator.OrchardCore.ViewModels
{
    public class CommentNotificationsContentViewModel : ShapeViewModel
    {
        public CommentNotificationsContentViewModel()
        {
            Metadata.Type = "BaseNotification";
        }

        public string TemplateName
        {
            get
            {
                return Metadata.Type;
            }

            set
            {
                Metadata.Type = value;
            }
        }

        public string User { get; set; }

        public string Message { get; set; }

        public dynamic ContentItem { get; set; }
        public string ContentData { get; set; }
    }
}
