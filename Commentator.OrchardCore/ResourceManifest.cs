﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Commentator.OrchardCore
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static readonly ResourceManifest manifest;

        static ResourceManagementOptionsConfiguration()
        {
            manifest = new ResourceManifest();

            manifest
                .DefineScript("Commentator-site-js")
                .SetUrl("~/Commentator.OrchardCore/js/site.min.js", "~/Commentator.OrchardCore/js/site.js")
                .SetVersion("1.0.0");

            manifest
                 .DefineScript("sweetalert-js")
                 .SetCdn("https://unpkg.com/sweetalert/dist/sweetalert.min.js");

            manifest
                 .DefineScript("Trumbowyg-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/trumbowyg.min.js");

            manifest
                 .DefineScript("Trumbowyg-giphy-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/giphy/trumbowyg.giphy.min.js");

            manifest
                 .DefineScript("Trumbowyg-emoji-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/emoji/trumbowyg.emoji.min.js");

            manifest
                 .DefineScript("Trumbowyg-fontfamily-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/fontfamily/trumbowyg.fontfamily.min.js");

            manifest
                 .DefineScript("CkEditor5-js")
                 .SetUrl("~/Commentator.OrchardCore/js/ckeditor.js")
                 .SetVersion("5.0.0");

            manifest
                 .DefineScript("Trumbowyg-fontsize-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/fontsize/trumbowyg.fontsize.min.js");

            manifest
               .DefineStyle("Commentator-site")
               .SetUrl("~/Commentator.OrchardCore/css/site.min.css", "~/Commentator.OrchardCore/css/site.css")
               .SetVersion("1.0.0");
            manifest
               .DefineStyle("CkEditor5-css")
               .SetUrl("~/Commentator.OrchardCore/css/ckeditor.css")
               .SetVersion("5.0.0");

            manifest
               .DefineStyle("Trumbowyg-css")
               .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/ui/trumbowyg.min.css")
               .SetVersion("2.21.0");

            manifest
               .DefineStyle("Trumbowyg-emoji-css")
               .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/emoji/ui/trumbowyg.emoji.min.css")
               .SetVersion("2.21.0");

            manifest
               .DefineStyle("Trumbowyg-giphy-css")
               .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/giphy/ui/trumbowyg.giphy.min.css")
               .SetVersion("2.21.0");
        }
        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(manifest);
        }
    }
}
