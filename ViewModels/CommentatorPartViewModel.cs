using Commentator.OrchardCore.Models;
using Commentator.OrchardCore.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;

namespace Commentator.OrchardCore.ViewModels
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
