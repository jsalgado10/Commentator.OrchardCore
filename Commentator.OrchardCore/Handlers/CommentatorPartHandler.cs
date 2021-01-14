using Commentator.OrchardCore.Models;
using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;

namespace Commentator.OrchardCore.Handlers
{
    public class CommentatorPartHandler : ContentPartHandler<CommentatorPart>
    {
        public override Task InitializingAsync(InitializingContentContext context, CommentatorPart part)
        {
            // By default Comments are shown.
            part.AllowComments = true;

            return Task.CompletedTask;
        }
    }
}