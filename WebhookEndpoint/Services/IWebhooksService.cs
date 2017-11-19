using System.Threading.Tasks;
using WebhookEndpoint.Models;

namespace WebhookEndpoint.Services
{
    public interface IWebhooksService
    {
        Task ProcessEventAsync(SfEvent sfEvent);
    }
}