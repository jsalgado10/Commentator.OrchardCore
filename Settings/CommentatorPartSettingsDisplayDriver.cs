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

                model.ShortName = settings.ShortName;
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

            if (await context.Updater.TryUpdateModelAsync(model, Prefix, m => m.ShortName))
            {
                context.Builder.WithSettings(new CommentatorPartSettings { ShortName = model.ShortName });
            }

            return Edit(contentTypePartDefinition, context.Updater);
        }
    }
}