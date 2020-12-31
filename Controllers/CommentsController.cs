using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Commentator.ViewModels;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using OrchardCore.Settings;
using YesSql;
using YesSql.Services;

namespace OrchardCore.Commentator.Controllers
{
    public class CommentsController : Controller
    {
        private readonly IContentManager contentManager;
        private readonly IContentDefinitionManager contentDefinitionManager;
        private readonly ISiteService siteService;
        private readonly ISession session;
        private readonly IContentItemDisplayManager contentItemDisplayManager;
        private readonly INotifier _notifier;
        private readonly IAuthorizationService authorizationService;
        private readonly IHtmlLocalizer H;
        private readonly IStringLocalizer S;
        private readonly IUpdateModelAccessor updateModelAccessor;
        private readonly ILogger logger;
        private readonly dynamic New;

        public CommentsController(
            IContentManager currentContentManager,
            IContentItemDisplayManager currentContentItemDisplayManager,
            IContentDefinitionManager currentContentDefinitionManager,
            ISiteService currentSiteService,
            INotifier notifier,
            ISession currentSession,
            IShapeFactory shapeFactory,
            ILogger<CommentsController> currentLogger,
            IHtmlLocalizer<CommentsController> htmlLocalizer,
            IStringLocalizer<CommentsController> stringLocalizer,
            IAuthorizationService currentAuthorizationService,
            IUpdateModelAccessor currentUpdateModelAccessor)
        {
            authorizationService = currentAuthorizationService;
            _notifier = notifier;
            contentItemDisplayManager = currentContentItemDisplayManager;
            session = currentSession;
            siteService = currentSiteService;
            contentManager = currentContentManager;
            contentDefinitionManager = currentContentDefinitionManager;
            updateModelAccessor = currentUpdateModelAccessor;

            H = htmlLocalizer;
            S = stringLocalizer;
            New = shapeFactory;
            logger = currentLogger;
        }

        public async Task<ActionResult> List(ListContentsViewModel model, PagerParameters pagerParameters, string contentTypeId = "")
        {
            if(model.PageCount == 0)
            {
                model.PageCount = 999;
            }

            List<ContentItem> contentItems;
            var siteSettings = await siteService.GetSiteSettingsAsync();
            var pager = new Pager(pagerParameters, model.PageCount);
            var query = await session.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "CommentPost" && x.Latest == true && x.Published == true)
                .ListAsync();

            /* Get all comments for Document */
            contentItems = query.Where(x => BelongsToDocument(x, contentTypeId)).ToList();

            /* Group By */
            switch (model.Options.GroupBy)
            {
                case ContentsGrouping.Thread:
                    /* Update Child Count */
                    foreach (var contentItem in contentItems)
                    {
                        string parentId = contentItem.ContentItemId;
                        contentItem.Content.CommentPost.ChildCount.Value = contentItems.Count(comment => IsChildOf(parentId, comment));
                    }
                    if (!model.OnlySubComments)
                    {
                        contentItems = contentItems.Where(x => IsParentComment(x)).ToList();
                    }
                    break;
                case ContentsGrouping.Single:
                    contentItems = contentItems.ToList();
                    break;
                default:
                    contentItems = contentItems.ToList();
                    break;
            }


            /* Get Only Sub Comments */
            if (model.OnlySubComments && !string.IsNullOrEmpty(model.ParentId))
            {
                contentItems = contentItems.Where(x => IsChildOf(model.ParentId, x)).ToList();
            }

            /* Order By */
            switch (model.Options.OrderBy)
            {
                case ContentsOrder.Date:
                    logger.LogInformation("Order By Date");
                    contentItems = contentItems.OrderByDescending(x => x.PublishedUtc).ToList();
                    break;
                case ContentsOrder.Rank:
                    logger.LogInformation("Order By Rank");
                    break;
                default:
                    logger.LogInformation("No Order");
                    contentItems = contentItems.OrderByDescending(x => x.PublishedUtc).ToList();
                    break;
            }

            for (int i = 0; i < contentItems.Count; i++)
            {
                contentItems[i] = await contentManager.LoadAsync(contentItems[i]);
            }

            /* Prepare Pager */
            var maxPagedCount = siteSettings.MaxPagedCount;
            if (maxPagedCount > 0 && pager.PageSize > maxPagedCount)
                pager.PageSize = maxPagedCount;

            var pagerShape = (await New.Pager(pager)).TotalItemCount(maxPagedCount > 0 ? maxPagedCount : contentItems.Count());
            var pageOfContentItems = contentItems.Skip(pager.GetStartIndex()).Take(model.PageCount);

            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(await contentItemDisplayManager.BuildDisplayAsync(contentItem, updateModelAccessor.ModelUpdater));
            }

            var viewModel = new ListContentsViewModel
            {
                ContentItems = contentItemSummaries,
                Options = model.Options,
                PageCount = model.PageCount,
                Pager = pagerShape
            };
            return PartialView(viewModel);
        }

        public async Task<IActionResult> Create(string contentType)
        {
            if (String.IsNullOrWhiteSpace(contentType))
            {
                return NotFound();
            }

            var contentItem = await contentManager.NewAsync(contentType);

            contentItem.Owner = User.Identity.Name;

            if (!await authorizationService.AuthorizeAsync(User, Permissions.AddCommentsAccess))
            {
                return Forbid();
            }

            var model = await contentItemDisplayManager.BuildEditorAsync(contentItem, updateModelAccessor.ModelUpdater, true);

            return PartialView(model);
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateAndPublishPOST(string contentType, string returnUrl)
        {
            if (!await authorizationService.AuthorizeAsync(User, Permissions.AddCommentsAccess))
            {
                return Forbid();
            }

            return await CreatePOST(contentType, returnUrl, async contentItem =>
             {
                 await contentManager.PublishAsync(contentItem);
             });
        }


        private async Task<IActionResult> CreatePOST(string id, string returnUrl, Func<ContentItem, Task> conditionallyPublish)
        {
            var contentItem = await contentManager.NewAsync(id);

            contentItem.Owner = User.Identity.Name;

            if (!await authorizationService.AuthorizeAsync(User, Permissions.AddCommentsAccess))
            {
                return Forbid();
            }

            var model = await contentItemDisplayManager.UpdateEditorAsync(contentItem, updateModelAccessor.ModelUpdater, true);

            if (!ModelState.IsValid)
            {
                session.Cancel();
                return PartialView(model);
            }

            await contentManager.CreateAsync(contentItem, VersionOptions.Draft);

            await conditionallyPublish(contentItem);

            return PartialView(model);
        }

        private bool BelongsToDocument(ContentItem item, string documentId)
        {
            dynamic content = item.Content;

            return content.CommentPost.CommentArticle.Text.Value == documentId;
        }

        private bool IsParentComment(ContentItem item)
        {
            dynamic content = item.Content;

            return content.CommentPost.CommentParent.Text.Value == "0";
        }

        private bool IsChildOf(string parentId, ContentItem comment)
        {
            string commentParent = comment.Content.CommentPost.CommentParent.Text.Value;

            return parentId == commentParent;
        }
    }
}
