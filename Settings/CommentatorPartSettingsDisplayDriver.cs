using System;
using System.Threading.Tasks;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Commentator.Models;

namespace OrchardCore.Commentator.Settings
{
    public class CommentatorPartSettingsDisplayDriver : ContentTypePartDefinitionDisplayDriver
    {
        public override IDisplayResult Edit(ContentTypePartDefinition contentTypePartDefinition)
        {
            if (!String.Equals(nameof(CommentatorPart), contentTypePartDefinition.PartDefinition.Name, StringComparison.Ordinal))
            {
                return null;
            }

            return Initialize<CommentatorPartSettingsViewModel>("CommentatorPartSettings_Edit", model =>
            {
                var settings = contentTypePartDefinition.GetSettings<CommentatorPartSettings>();

                model.OrderBy = settings.OrderBy;
                model.GroupBy = settings.GroupBy;
                model.Editor = settings.Editor;
                model.CommentsPerPage = settings.CommentsPerPage;
                model.CommentatorPartSettings = settings;

            }).Location("Content");
        }

        public override async Task<IDisplayResult> UpdateAsync(ContentTypePartDefinition contentTypePartDefinition, UpdateTypePartEditorContext context)
        {
            if (!String.Equals(nameof(CommentatorPart), contentTypePartDefinition.PartDefinition.Name, StringComparison.Ordinal))
            {
                return null;
            }

            var model = new CommentatorPartSettingsViewModel();

            if (await context.Updater.TryUpdateModelAsync(model, Prefix, m => m.OrderBy, m => m.GroupBy, m => m.CommentsPerPage, m => m.Editor))
            {
                context.Builder.WithSettings(new CommentatorPartSettings
                {
                    OrderBy = model.OrderBy,
                    GroupBy = model.GroupBy,
                    Editor = model.Editor,
                    CommentsPerPage = model.CommentsPerPage
                });
            }

            return Edit(contentTypePartDefinition, context.Updater);
        }
    }
}