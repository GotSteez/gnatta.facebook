namespace Gnatta.Facebook.WebhookBackup.Models
{
    public class FacebookPaging
    {
        public FacebookPagingCursors cursors { get; set; }
    }

    public class FacebookPagingCursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }
}