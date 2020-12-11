using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.Commentator.Controllers;
using OrchardCore.Commentator.Drivers;
using OrchardCore.Commentator.Handlers;
using OrchardCore.Commentator.Models;
using OrchardCore.Commentator.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Routing;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.ResourceManagement;
using OrchardCore.Security.Permissions;
using System;

namespace OrchardCore.Commentator
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IResourceManifestProvider, ResourceManifest>();
            services.AddContentPart<CommentatorPart>()
                .UseDisplayDriver<CommentatorPartDisplayDriver>()
                .AddHandler<CommentatorPartHandler>();
            services.AddScoped<IContentTypePartDefinitionDisplayDriver, CommentatorPartSettingsDisplayDriver>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<IPermissionProvider, Permissions>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var itemController = typeof(CommentsController).ControllerName();

            routes.MapAreaControllerRoute(
                name: "DisplayArticleComments",
                areaName: "OrchardCore.Commentator",
                pattern: "comments/{contentTypeId}",
                defaults: new { controller = itemController, action = nameof(CommentsController.List) }
            );

            routes.MapAreaControllerRoute(
                name: "DisplayArticleComments",
                areaName: "OrchardCore.Commentator",
                pattern: "comments/Add/{contentType}",
                defaults: new { controller = itemController, action = nameof(CommentsController.Create) }
            );
        }
    }
}
