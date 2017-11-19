using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace WebhookEndpoint.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHostingEnvironment _environment;

        public HealthCheckService(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<HealthInfo> GetHealthAsync()
        {
            string instanceId = "local";
            if (_environment.IsProduction())
            {
                using (var client = new HttpClient())
                {
                    instanceId = await client.GetStringAsync("http://169.254.169.254/latest/meta-data/instance-ud");
                }
            }

            return new HealthInfo
            {
                InstanceId = instanceId,
                Environment = _environment.EnvironmentName
            };
        }
    }

    public class HealthInfo
    {
        public string InstanceId { get; set; }

        public string Environment { get; set; }        
    }
}