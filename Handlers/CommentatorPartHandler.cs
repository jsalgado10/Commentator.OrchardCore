using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Commentator.Models;
using System.Threading.Tasks;

namespace OrchardCore.Commentator.Handlers
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