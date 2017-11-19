using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShareFile.Api.Client;
using ShareFile.Api.Models;
using WebhookEndpoint.Models;

namespace WebhookEndpoint.Services
{
    public class WebhooksService
    {
        private readonly IShareFileClientFactory _sfClientFactory;
        private readonly string _queueUrl;
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<WebhooksService> _logger;
        public WebhooksService(IShareFileClientFactory clientFactory, IConfiguration configuration, IAmazonSQS sqsClient, ILogger<WebhooksService> logger)
        {
            _sfClientFactory = clientFactory;
            _sqsClient = sqsClient;
            _queueUrl = configuration.GetSection("AWS")["ProcessingQueue"];
            _logger = logger;
        }

        public async Task ProcessEventAsync(SfEvent sfEvent)
        {
            try
            {
                var sfClient = await _sfClientFactory.GetClientAsync();

                var fileUrl = sfClient.Items.GetEntityUriFromId(sfEvent.Event.Resource.Id);
                var file = await sfClient.Items.Get(fileUrl).ExecuteAsync();

                var share = await GetShareAsync(sfClient, file);

                await Enqueue(file, share);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception processing upload event");
            }
        }

        private Task<Share> GetShareAsync(IShareFileClient client, Item file)
        {
            var share = new Share
            {
                Items = new List<Item> { file },
                RequireUserInfo = false,
                RequireLogin = false,
                ShareType = ShareType.Send,
                Title = $"Share for {file.Name}",
                ExpirationDate = DateTime.Now.AddDays(10),
                MaxDownloads = -1
            };

            return client.Shares.Create(share).ExecuteAsync();
        }

        private async Task Enqueue(Item file, Share share)
        {
            var message = new 
            {
                ItemId = file.Id,
                ItemName = file.Name,
                ShareLink = share.Uri
            };

            var messageJson = JsonConvert.SerializeObject(message);

            var response = await _sqsClient.SendMessageAsync(_queueUrl, messageJson);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Enqueue to {_queueUrl} failed;");
            }
        }
    }
}