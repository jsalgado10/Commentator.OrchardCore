using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;
using System.Threading.Tasks;
using System;
using OrchardCore.Security.Services;

namespace OrchardCore.Commentator
{
    public class Migrations : DataMigration
    {
        private readonly IRecipeMigrator recipeMigrator;

        public Migrations(IRecipeMigrator currentRecipeMigrator)
        {
            recipeMigrator = currentRecipeMigrator;
        }

        public async Task<int> CreateAsync()
        {
            await recipeMigrator.ExecuteAsync("commentator.recipe.json", this);

            return 1;
        }
    }
}