using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleNotificationService;

namespace FileUploader
{
    public class SnsService
    {
        private IAmazonSimpleNotificationService _snsClient;

        public SnsService(RegionEndpoint region)
        {
            _snsClient = new AmazonSimpleNotificationServiceClient(region);
        }

        public async Task<bool> PublishAsync(string topicArn, string fileUrl, string fileName)
        {
            var message = $"Download file {fileName} at this link: {fileUrl}";
            var response = await _snsClient.PublishAsync(topicArn, message);

            return response.HttpStatusCode == HttpStatusCode.OK;
        }
    }
}