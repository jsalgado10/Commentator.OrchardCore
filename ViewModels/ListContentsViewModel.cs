using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrchardCore.Commentator.ViewModels
{
    public class ListContentsViewModel
    {
        public ListContentsViewModel()
        {
            Options = new ContentOptions();
        }

        public int PageCount { get; set; }

        public string ParentId { get; set; }

        public bool OnlySubComments { get; set; }

        public ContentOptions Options { get; set; }

        [BindNever]
        public List<dynamic> ContentItems { get; set; }

        [BindNever]
        public dynamic Pager { get; set; }
    }

    public class ContentOptions
    {
        public ContentOptions()
        {
            OrderBy = ContentsOrder.Date;
            GroupBy = ContentsGrouping.Thread;
        }

        public ContentsOrder OrderBy { get; set; }

        public ContentsGrouping GroupBy { get; set; }
    }

    public enum ContentsOrder
    {
        Date,
        Rank
    }

    public enum ContentsGrouping
    {
        Thread,
        Single
    }
}
