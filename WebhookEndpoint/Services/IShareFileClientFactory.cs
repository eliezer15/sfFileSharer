using System.Threading.Tasks;
using ShareFile.Api.Client;

namespace WebhookEndpoint.Services
{
    public interface IShareFileClientFactory
    {
        Task<IShareFileClient> GetClientAsync();
    }
}