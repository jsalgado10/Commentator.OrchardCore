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
using System.Net.Mime;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement.ModelBinding;

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

    public static async Task<IEnumerable<dynamic>> GetContentItemCommentsAsync(this IOrchardHelper orchardHelper, string contentTypeId)
    {
        List<ContentItem> contentItems;
        var session = orchardHelper.HttpContext.RequestServices.GetService<ISession>();
        var contentManager = orchardHelper.HttpContext.RequestServices.GetService<IContentManager>();
        var contentItemDisplayManager = orchardHelper.HttpContext.RequestServices.GetService<IContentItemDisplayManager>();
        var updateModelAccessor = orchardHelper.HttpContext.RequestServices.GetService<IUpdateModelAccessor>();

        var query = await session.Query<ContentItem, ContentItemIndex>()
            .Where(x => x.ContentType == "CommentPost" && x.Latest == true && x.Published == true)
            .ListAsync();

        contentItems = query.Where(x => BelongsToDocument(x, contentTypeId)).ToList();

        for (int i = 0; i < contentItems.Count; i++)
        {
            contentItems[i] = await contentManager.LoadAsync(contentItems[i]);
        }

        var contentItemSummaries = new List<dynamic>();
        foreach (var contentItem in contentItems)
        {
            contentItemSummaries.Add(await contentItemDisplayManager.BuildDisplayAsync(contentItem, updateModelAccessor.ModelUpdater));
        }

        return contentItemSummaries;
    }
}
