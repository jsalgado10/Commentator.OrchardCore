using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OrchardCore.Commentator.Settings
{
    public class CommentatorPartSettingsViewModel
    {
        public string ShortName { get; set; }

        [BindNever]
        public CommentatorPartSettings CommentatorPartSettings { get; set; }
    }
}
