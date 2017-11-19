using System.Threading.Tasks;

namespace WebhookEndpoint.Services
{
    public interface IHealthCheckService
    {
        Task<HealthInfo> GetHealthAsync();
    }
}