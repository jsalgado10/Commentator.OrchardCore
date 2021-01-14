using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.Users;

namespace Commentator.OrchardCore.Services
{
    public interface INotificationService
    {
        Task SendNotification(ContentItem item);
    }
}