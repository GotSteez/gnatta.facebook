using System.Collections.Generic;

namespace Gnatta.Facebook.WebhookBackup.Models
{
    public class FacebookWebhook
    {
        public FacebookWebhookEntry[] entry { get; set; }

        public string @object = "page";
    }

    public class FacebookWebhookEntry
    {
        public string id { get; set; }
        public long time { get; set; }
        public IEnumerable<FacebookWebhookChange> changes { get; set; }
    }

    // Conversations 
    public class FacebookWebhookConversationChange : FacebookWebhookChange
    {
        public string field = "conversations";
        public FacebookWebhookConversationValue value { get; set; }
    }

    public class FacebookWebhookConversationValue
    {
        public string thread_id { get; set; }
        public long page_id { get; set; }
        public string thread_key { get; set; }
    }
    
    // Feed
    public class FacebookWebhookFeedChange : FacebookWebhookChange
    {
        public string field = "feed";
        public FacebookWebhookFeedValue value { get; set; }
    }

    public class FacebookWebhookFeedValue
    {
        public string item { get; set; }
        
        public string post_id { get; set; }
        
        public string verb { get; set; }
        
        public int published { get; set; }
        
        public long created_time { get; set; }
        
        public string message { get; set; }
        
        public FacebookFromDetails from { get; set; }
    } 


    public interface FacebookWebhookChange
    {
    }
}
