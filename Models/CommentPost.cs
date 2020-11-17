using System;
using System.Collections.Generic;
using System.Text;

namespace OrchardCore.Commentator.Models
{
    public class CommentPost
    {
        public string ArticleId { get; set; }
        public string ParentId { get; set; }
        public string CommentContent { get; set; }
        public string CommentBy { get; set; }
        public DateTime CommentDateTime { get; set; }
        public int CommentStatus { get; set; }
        public string CommentTitle { get; set; }
    }
}
