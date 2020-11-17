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
        private readonly ILogger _logger;
        private readonly dynamic New;

        public CommentsController(
            IContentManager currentContentManager,
            IContentItemDisplayManager currentContentItemDisplayManager,
            IContentDefinitionManager currentContentDefinitionManager,
            ISiteService currentSiteService,
            INotifier notifier,
            ISession currentSession,
            IShapeFactory shapeFactory,
            ILogger<CommentsController> logger,
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
            _logger = logger;
        }

        public async Task<ActionResult> List(ListContentsViewModel model, PagerParameters pagerParameters, string contentTypeId = "")
        {
            Console.WriteLine($@"Article ID {contentTypeId}");
            List<ContentItem> contentItems;
            contentItems = (await session.Query<ContentItem, ContentItemIndex>()
                .Where(x => x.ContentType == "CommentPost" && x.Latest == true && x.Published == true)
                .OrderByDescending(x => x.PublishedUtc)
                .ListAsync()).ToList();
            
            /* Filter only comments for contentTypeId */
            contentItems = contentItems.Where(x => BelongsToDocument(x, contentTypeId)).ToList();

            for (int i = 0; i < contentItems.Count; i++)
            {
                contentItems[i] = await contentManager.LoadAsync(contentItems[i]);
            }

            //We prepare the content items SummaryAdmin shape
            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in contentItems)
            {
                contentItemSummaries.Add(await contentItemDisplayManager.BuildDisplayAsync(contentItem, updateModelAccessor.ModelUpdater));
            }
            //var siteSettings = await siteService.GetSiteSettingsAsync();
            //var pager = new Pager(pagerParameters, siteSettings.PageSize);

            //var query = session.Query<ContentItem, ContentItemIndex>();

            //if (!string.IsNullOrEmpty(model.Options.DisplayText))
            //{
            //    query = query.With<ContentItemIndex>(x => x.DisplayText.Contains(model.Options.DisplayText));
            //}

            //switch (model.Options.ContentsStatus)
            //{
            //    case ContentsStatus.Published:
            //        query = query.With<ContentItemIndex>(x => x.Published);
            //        break;
            //    case ContentsStatus.Draft:
            //        query = query.With<ContentItemIndex>(x => x.Latest && !x.Published);
            //        break;
            //    case ContentsStatus.AllVersions:
            //        query = query.With<ContentItemIndex>(x => x.Latest);
            //        break;
            //    default:
            //        query = query.With<ContentItemIndex>(x => x.Latest);
            //        break;
            //}

            //if (model.Options.ContentsStatus == ContentsStatus.Owner)
            //{
            //    query = query.With<ContentItemIndex>(x => x.Owner == HttpContext.User.Identity.Name);
            //}

            //if (!string.IsNullOrEmpty(contentTypeId))
            //{
            //    model.Options.SelectedContentType = contentTypeId;
            //}

            //IEnumerable<ContentTypeDefinition> contentTypeDefinitions = new List<ContentTypeDefinition>();
            //if (!string.IsNullOrEmpty(model.Options.SelectedContentType))
            //{
            //    var contentTypeDefinition = contentDefinitionManager.GetTypeDefinition(model.Options.SelectedContentType);

            //    if (contentTypeDefinition == null)
            //    {
            //        return NotFound();
            //    }

            //    contentTypeDefinitions = contentTypeDefinitions.Append(contentTypeDefinition);

            //    // We display a specific type even if it's not listable so that admin pages
            //    // can reuse the Content list page for specific types.
            //    query = query.With<ContentItemIndex>(x => x.ContentType == model.Options.SelectedContentType);

            //    // Allows non creatable types to be created by another admin page.
            //    if (model.Options.CanCreateSelectedContentType)
            //    {
            //        model.Options.CreatableTypes = new List<SelectListItem>
            //        {
            //            new SelectListItem(new LocalizedString(contentTypeDefinition.DisplayName, contentTypeDefinition.DisplayName).Value, contentTypeDefinition.Name)
            //        };
            //    }
            //}
            //else
            //{
            //    contentTypeDefinitions = contentDefinitionManager.ListTypeDefinitions();

            //    var listableTypes = (await GetListableTypesAsync()).Select(t => t.Name).ToArray();
            //    if (listableTypes.Any())
            //    {
            //        query = query.With<ContentItemIndex>(x => x.ContentType.IsIn(listableTypes));
            //    }
            //}

            //switch (model.Options.OrderBy)
            //{
            //    case ContentsOrder.Modified:
            //        query = query.OrderByDescending(x => x.ModifiedUtc);
            //        break;
            //    case ContentsOrder.Published:
            //        query = query.OrderByDescending(cr => cr.PublishedUtc);
            //        break;
            //    case ContentsOrder.Created:
            //        query = query.OrderByDescending(cr => cr.CreatedUtc);
            //        break;
            //    case ContentsOrder.Title:
            //        query = query.OrderBy(cr => cr.DisplayText);
            //        break;
            //    default:
            //        query = query.OrderByDescending(cr => cr.ModifiedUtc);
            //        break;
            //}

            //// Allow parameters to define creatable types.
            //if (model.Options.CreatableTypes == null)
            //{
            //    var contentTypes = contentTypeDefinitions.Where(ctd => ctd.GetSettings<ContentTypeSettings>().Creatable).OrderBy(ctd => ctd.DisplayName);
            //    var creatableList = new List<SelectListItem>();
            //    if (contentTypes.Any())
            //    {
            //        foreach (var contentTypeDefinition in contentTypes)
            //        {
            //            creatableList.Add(new SelectListItem(new LocalizedString(contentTypeDefinition.DisplayName, contentTypeDefinition.DisplayName).Value, contentTypeDefinition.Name));
            //        }
            //    }

            //    model.Options.CreatableTypes = creatableList;
            //}

            //// Invoke any service that could alter the query
            //await contentAdminFilters.InvokeAsync((filter, query, model, pagerParameters, updateModel) => filter.FilterAsync(query, model, pagerParameters, updateModel), query, model, pagerParameters, _updateModelAccessor.ModelUpdater, _logger);

            //var maxPagedCount = siteSettings.MaxPagedCount;
            //if (maxPagedCount > 0 && pager.PageSize > maxPagedCount)
            //    pager.PageSize = maxPagedCount;

            ////We prepare the pager
            //var routeData = new RouteData();
            //routeData.Values.Add("DisplayText", model.Options.DisplayText);

            //var pagerShape = (await New.Pager(pager)).TotalItemCount(maxPagedCount > 0 ? maxPagedCount : await query.CountAsync()).RouteData(routeData);
            //var pageOfContentItems = await query.Skip(pager.GetStartIndex()).Take(pager.PageSize).ListAsync();

            ////We prepare the content items SummaryAdmin shape
            //var contentItemSummaries = new List<dynamic>();
            //foreach (var contentItem in pageOfContentItems)
            //{
            //    contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, _updateModelAccessor.ModelUpdater, "SummaryAdmin"));
            //}

            ////We populate the SelectLists
            //model.Options.ContentStatuses = new List<SelectListItem>() {
            //    new SelectListItem() { Text = S["Latest"], Value = nameof(ContentsStatus.Latest) },
            //    new SelectListItem() { Text = S["Owned by me"], Value = nameof(ContentsStatus.Owner) },
            //    new SelectListItem() { Text = S["Published"], Value = nameof(ContentsStatus.Published) },
            //    new SelectListItem() { Text = S["Unpublished"], Value = nameof(ContentsStatus.Draft) },
            //    new SelectListItem() { Text = S["All versions"], Value = nameof(ContentsStatus.AllVersions) }
            //};

            //model.Options.ContentSorts = new List<SelectListItem>() {
            //    new SelectListItem() { Text = S["Recently created"], Value = nameof(ContentsOrder.Created) },
            //    new SelectListItem() { Text = S["Recently modified"], Value = nameof(ContentsOrder.Modified) },
            //    new SelectListItem() { Text = S["Recently published"], Value = nameof(ContentsOrder.Published) },
            //    new SelectListItem() { Text = S["Title"], Value = nameof(ContentsOrder.Title) }
            //};

            //model.Options.ContentsBulkAction = new List<SelectListItem>() {
            //    new SelectListItem() { Text = S["Publish Now"], Value = nameof(ContentsBulkAction.PublishNow) },
            //    new SelectListItem() { Text = S["Unpublish"], Value = nameof(ContentsBulkAction.Unpublish) },
            //    new SelectListItem() { Text = S["Delete"], Value = nameof(ContentsBulkAction.Remove) }
            //};

            //var ContentTypeOptions = (await GetListableTypesAsync())
            //    .Select(ctd => new KeyValuePair<string, string>(ctd.Name, ctd.DisplayName))
            //    .ToList().OrderBy(kvp => kvp.Value);

            //model.Options.ContentTypeOptions = new List<SelectListItem>();
            //model.Options.ContentTypeOptions.Add(new SelectListItem() { Text = S["All content types"], Value = "" });
            //foreach (var option in ContentTypeOptions)
            //{
            //    model.Options.ContentTypeOptions.Add(new SelectListItem() { Text = option.Value, Value = option.Key });
            //}

            //var viewModel = new ListContentsViewModel
            //{
            //    ContentItems = contentItemSummaries,
            //    Pager = pagerShape,
            //    Options = model.Options
            //};
            var viewModel = new ListContentsViewModel
            {
                ContentItems = contentItemSummaries
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
    }
}
