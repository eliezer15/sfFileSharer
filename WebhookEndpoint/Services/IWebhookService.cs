using System.Threading.Tasks;
using WebhookEndpoint.Models;

namespace WebhookEndpoint.Services
{
    public interface IWebhookService
    {
        Task ProcessEventAsync(SfEvent sfEvent);
    }
}