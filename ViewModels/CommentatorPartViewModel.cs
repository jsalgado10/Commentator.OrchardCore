using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;
using OrchardCore.Commentator.Models;
using OrchardCore.Commentator.Settings;

namespace OrchardCore.Commentator.ViewModels
{
    public class CommentatorPartViewModel
    {
        public string ShortName { get; set; }

        public bool AllowComments { get; set; }

        [BindNever]
        public ContentItem ContentItem { get; set; }

        [BindNever]
        public CommentatorPart CommentatorPart { get; set; }

        [BindNever]
        public CommentatorPartSettings Settings { get; set; }
    }
}
