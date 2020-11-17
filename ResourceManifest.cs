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
                .DefineScript("toastr-js")
                .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js")
                .SetVersion("2.1.1");

            manifest
                 .DefineScript("sweetalert-js")
                 .SetCdn("https://unpkg.com/sweetalert/dist/sweetalert.min.js");

            manifest
               .DefineStyle("Commentator-site")
               .SetUrl("~/OrchardCore.Commentator/css/site.min.css", "~/OrchardCore.Commentator/css/site.css")
               .SetVersion("1.0.0");

            manifest
               .DefineStyle("toastr-css")
               .SetCdn("https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css")
               .SetVersion("1.0.0");
        }
    }
}
