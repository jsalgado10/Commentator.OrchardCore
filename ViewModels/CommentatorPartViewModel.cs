using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;
using OrchardCore.Commentator.Models;
using OrchardCore.Commentator.Settings;

namespace OrchardCore.Commentator.ViewModels
{
    public class CommentatorPartViewModel
    {
        public bool AllowComments { get; set; }
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
        public string Editor { get; set; }
        public int? CommentsPerPage { get; set; }

        [BindNever]
        public ContentItem ContentItem { get; set; }

        [BindNever]
        public CommentatorPart CommentatorPart { get; set; }

        [BindNever]
        public CommentatorPartSettings Settings { get; set; }
    }
}
