using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShareFile.Api.Client;
using ShareFile.Api.Models;

namespace FileUploader
{
    public class ServiceHost
    {
        private IConfigurationRoot _configuration;
        private SqsService _sqsService;
        private S3Service _s3Service;
        private SnsService _snsService;

        public ServiceHost()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var regionName = _configuration.GetSection("AWS")["Region"];
            System.Console.WriteLine("Working on region " + regionName);           
            var region = RegionEndpoint.GetBySystemName(regionName);

            _snsService = new SnsService(region);
            _sqsService = new SqsService(region);
            _s3Service = new S3Service(region);
        }

        public async Task Run()
        {
            while (true)
            {
                System.Console.WriteLine("Pulling from queue...");
                var message = await _sqsService.DequeueAsync(_configuration["SqsQueueUrl"]);
                if (message == null)
                {
                    await Task.Delay(5000);
                    continue;
                }

                var fileStream = await DownloadFromShareFileAsync(message.ShareLink, message.ItemName);
                
                bool fileUploaded = await _s3Service.UploadAsync(_configuration["S3Bucket"], message.ItemName, fileStream);
                bool topicSent = await _snsService.PublishAsync(_configuration["SnsTopicArn"], message.ShareLink, message.ItemName);

                Console.WriteLine($"Finished processing message; File uploaded: {fileUploaded}, File Sent: {topicSent}");

                if (topicSent)
                {
                    await _sqsService.DeleteMessageAsync(_configuration["SqsQueueUrl"], message.ReceiptHandle);
                }
            }
        }

        private async Task<string> DownloadFromShareFileAsync(string shareLink, string itemName)
        {
            var uri = new Uri(shareLink);
            var sfClient = new ShareFileClient($"https://{uri.Authority}/sf/v3");

            // Extract shareId from path /d-<shareid>
            var shareId = uri.AbsolutePath.Substring(3);
            System.Console.WriteLine("ShareId: " + shareId);

            var shareEntityUrl = sfClient.Shares.GetEntityUriFromId(shareId);
            var response = await sfClient.Shares.Download(shareEntityUrl, redirect: false).ExecuteAsync();

            DownloadSpecification sfDownloadSpec = null;
            using (var stream = new StreamReader(response))
            {
                var bodyJson = await stream.ReadToEndAsync();
                sfDownloadSpec = JsonConvert.DeserializeObject<DownloadSpecification>(bodyJson);
            }
            
            Directory.CreateDirectory("temp");
            var fileName = $"temp/{itemName}";

            var webClient = new WebClient();
            webClient.DownloadFile(sfDownloadSpec.DownloadUrl, fileName);
            
            return fileName;
        }
    }
}