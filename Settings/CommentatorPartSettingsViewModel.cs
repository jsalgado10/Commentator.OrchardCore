using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Commentator.OrchardCore.Settings
{
    public class CommentatorPartSettingsViewModel
    {
        public string OrderBy { get; set; }
        public string GroupBy { get; set; }
        public string Editor { get; set; }
        public int CommentsPerPage { get; set; }

        [BindNever]
        public CommentatorPartSettings CommentatorPartSettings { get; set; }
    }
}
