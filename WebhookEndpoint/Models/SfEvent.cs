using System;

namespace WebhookEndpoint.Models
{
    public class SfEvent
    {
        public string WebhookSubscriptionId { get; set; }

        public Event Event { get; set; }
    }

    public class Event 
    {
        public DateTime Timestamp { get; set; }
        
        public string ResourceType { get; set; }
        public Resource Resource { get; set; }
    }

    public class Resource 
    {
        public string Id { get; set; }
    }
}