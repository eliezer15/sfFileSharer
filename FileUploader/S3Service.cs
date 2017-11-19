using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace FileUploader
{
    public class S3Service
    {
        private IAmazonS3 _s3Client;
        public S3Service(RegionEndpoint region)
        {
            _s3Client = new AmazonS3Client(region);
        }

        public async Task<bool> UploadAsync(string bucketName, string itemName, string localFileName)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"file-backup/{itemName}"
            };

            using (FileStream stream = new FileStream(localFileName, FileMode.Open))
            {
                putRequest.InputStream = stream;
                var response = await _s3Client.PutObjectAsync(putRequest);

                return response.HttpStatusCode == HttpStatusCode.OK;
            }

            return false;
        }
    }
}