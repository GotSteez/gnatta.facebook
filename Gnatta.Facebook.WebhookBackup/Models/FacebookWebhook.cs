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
        public FacebookWebhookConversationChange[] changes { get; set; }
    }

    public class FacebookWebhookConversationChange
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
}
