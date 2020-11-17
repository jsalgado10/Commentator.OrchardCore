using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.Commentator.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Mvc.Utilities;

namespace OrchardCore.Commentator.Controllers
{
    [Route("api/comments")]
    [Produces("application/json")]
    [ApiController]
    public class ApiCommentsController : Controller
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IContentManager contentManager;

        public ApiCommentsController(IAuthorizationService currentAuthorizationService, IContentManager currentContentManager)
        {
            authorizationService = currentAuthorizationService;
            contentManager = currentContentManager;
        }

        //[http]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            if (!await authorizationService.AuthorizeAsync(User, Permissions.AddCommentsAccess))
            {
                return this.ChallengeOrForbid();
            }
            return Ok(new { result = "succes" });
        }
    }
}
