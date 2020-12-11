using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.ResourceManagement;

namespace OrchardCore.Commentator
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(IResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest
                .DefineScript("Commentator-site-js")
                .SetUrl("~/OrchardCore.Commentator/js/site.min.js", "~/OrchardCore.Commentator/js/site.js")
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
                 .SetUrl("~/OrchardCore.Commentator/js/ckeditor.js")
                 .SetVersion("5.0.0");

            manifest
                 .DefineScript("Trumbowyg-fontsize-js")
                 .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/Trumbowyg/2.21.0/plugins/fontsize/trumbowyg.fontsize.min.js");

            manifest
               .DefineStyle("Commentator-site")
               .SetUrl("~/OrchardCore.Commentator/css/site.min.css", "~/OrchardCore.Commentator/css/site.css")
               .SetVersion("1.0.0");
            manifest
               .DefineStyle("CkEditor5-css")
               .SetUrl("~/OrchardCore.Commentator/css/ckeditor.css")
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
    }
}
