using Commentator.OrchardCore.Controllers;
using Commentator.OrchardCore.Drivers;
using Commentator.OrchardCore.Handlers;
using Commentator.OrchardCore.Models;
using Commentator.OrchardCore.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentManagement.Routing;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.ResourceManagement;
using OrchardCore.Security.Permissions;
using OrchardCore.Users.Models;
using System;

namespace Commentator.OrchardCore
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
            services.AddScoped<IDisplayDriver<User>, UserProfileCommentatorDisplayDriver>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            var itemController = typeof(CommentsController).ControllerName();

            routes.MapAreaControllerRoute(
                name: "DisplayArticleComments",
                areaName: "Commentator.OrchardCore",
                pattern: "comments/{contentTypeId}",
                defaults: new { controller = itemController, action = nameof(CommentsController.List) }
            );

            routes.MapAreaControllerRoute(
                name: "AddArticleComments",
                areaName: "Commentator.OrchardCore",
                pattern: "comments/Add/{contentType}",
                defaults: new { controller = itemController, action = nameof(CommentsController.Create) }
            );
        }
    }
}
