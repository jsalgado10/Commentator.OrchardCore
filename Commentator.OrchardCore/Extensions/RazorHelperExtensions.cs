using OrchardCore;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using YesSql;
using System.Threading.Tasks;
using OrchardCore.ContentManagement.Records;
using System.Linq;

public static class RazorHelperExtensions
{
    public static async Task<int> CommentCount(this IOrchardHelper orchardHelper, ContentItem contentItem)
    {
        var session = orchardHelper.HttpContext.RequestServices.GetService<ISession>();
        IEnumerable<ContentItem> comments = await session.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "CommentPost" && x.Latest == true && x.Published == true)
                .ListAsync();
        int commentCount = comments.Count(x => BelongsToDocument(x, contentItem.ContentItemId));

        return commentCount;
    }

    private static bool BelongsToDocument(ContentItem item, string documentId)
    {
        dynamic content = item.Content;

        return content.CommentPost.CommentArticle.Text.Value == documentId;
    }
}
