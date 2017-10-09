using System;

namespace Gnatta.Facebook.WebhookBackup.Models
{
    public class FacebookConversationPage
    {
        public FacebookConversationPageItem[] data { get; set; }

        public FacebookPaging paging { get; set; }
    }

    public class FacebookConversationPageItem
    {
        public string id { get; set; }

        public DateTime updated_time { get; set; }
    }
}
