namespace FileUploader.Models
{
    public class SfEventQueueMessage
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string ShareLink { get; set; }
        public string ReceiptHandle { get; set; }
    }
}