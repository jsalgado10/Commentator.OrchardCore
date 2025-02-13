﻿using System.Linq;
using System.Threading.Tasks;
using Commentator.OrchardCore.Models;
using Commentator.OrchardCore.Settings;
using Commentator.OrchardCore.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;

namespace Commentator.OrchardCore.Drivers
{
    public class CommentatorPartDisplayDriver : ContentPartDisplayDriver<CommentatorPart>
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public CommentatorPartDisplayDriver(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public override IDisplayResult Display(CommentatorPart commentatorPart, BuildPartDisplayContext context)
        {
            return Combine(
                Initialize<CommentatorPartViewModel>("CommentatorPart", m => BuildViewModel(m, commentatorPart))
                    .Location("Detail", "Content:20"),
                Initialize<CommentatorPartViewModel>("CommentatorPart_Summary", m => BuildViewModel(m, commentatorPart))
                    .Location("Summary", "Meta:5")
            );
        }

        public override IDisplayResult Edit(CommentatorPart commentatorPart, BuildPartEditorContext context)
        {
            return Initialize<CommentatorPartViewModel>("CommentatorPart_Edit", m => BuildViewModel(m, commentatorPart));
        }

        public override async Task<IDisplayResult> UpdateAsync(CommentatorPart model, UpdatePartEditorContext context)
        {
            var settings = GetCommentatorPartSettings(model);

            await context.Updater.TryUpdateModelAsync(model, Prefix, t => t.AllowComments);

            return Edit(model, context);
        }

        public async Task<CommentatorPartSettings> GetCommentatorPartSettings(CommentatorPart part)
        {
            var contentTypeDefinition = await _contentDefinitionManager.GetTypeDefinitionAsync(part.ContentItem.ContentType);
            var contentTypePartDefinition = contentTypeDefinition.Parts.FirstOrDefault(p => p.PartDefinition.Name == nameof(CommentatorPart));
            var settings = contentTypePartDefinition.GetSettings<CommentatorPartSettings>();

            return settings;
        }

        private Task BuildViewModel(CommentatorPartViewModel model, CommentatorPart part)
        {
            var settings = GetCommentatorPartSettings(part).Result;

            model.ContentItem = part.ContentItem;
            model.OrderBy = settings.OrderBy;
            model.GroupBy = settings.GroupBy;
            model.Editor = settings.Editor;
            model.CommentsPerPage = settings.CommentsPerPage;
            model.AllowComments = part.AllowComments;
            model.CommentatorPart = part;
            model.Settings = settings;

            return Task.CompletedTask;
        }
    }
}
