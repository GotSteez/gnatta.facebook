using System;

namespace Gnatta.Facebook.WebhookBackup.Models
{
    public class FacebookFeedResult
    {
        public FacebookPageFeedItem[] data { get; set; }
        
        public FacebookPaging paging { get; set; }
    }

    public class FacebookPageFeedItem
    {
        public string id { get; set; }
        
        public string message { get; set; }
        
        public DateTime created_time { get; set; }
        
        public DateTime updated_time { get; set; }
        
        public string type { get; set; }
        
        public string status_type { get; set; }
        
        public FacebookFromDetails from { get; set; }
    }
}