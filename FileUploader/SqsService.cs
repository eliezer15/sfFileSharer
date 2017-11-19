using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FileUploader.Models;
using Newtonsoft.Json;

namespace FileUploader
{
    public class SqsService
    {
        private IAmazonSQS _sqsClient;
        public SqsService(RegionEndpoint region)
        {
            _sqsClient = new AmazonSQSClient(region);
        }

        public async Task<SfEventQueueMessage> DequeueAsync(string queueUrl)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                WaitTimeSeconds = 20,
            };
            try
            {
                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest);

                if (response.Messages.Count > 0)
                {
                    System.Console.WriteLine("Sqs: A message was retrieved");
                    var message = response.Messages.First();

                    var queueMessage = JsonConvert.DeserializeObject<SfEventQueueMessage>(message.Body);
                    queueMessage.ReceiptHandle = message.ReceiptHandle;

                    return queueMessage;
                }

                System.Console.WriteLine("Sqs: No messages found");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception " + ex.Message);
            }
            return null;
        }

        public async Task DeleteMessageAsync(string queueUrl, string receiptHandle)
        {
            var result = await _sqsClient.DeleteMessageAsync(queueUrl, receiptHandle);
            if (result.HttpStatusCode != HttpStatusCode.OK)
            {
                System.Console.WriteLine("Error deleting message");
            }
        }
    }
}