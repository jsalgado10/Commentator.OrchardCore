using System.Threading.Tasks;
using OrchardCore.ContentManagement;
using OrchardCore.Users;

namespace OrchardCore.Commentator.Services
{
    public interface INotificationService
    {
        Task SendNotification(ContentItem item);
    }
}