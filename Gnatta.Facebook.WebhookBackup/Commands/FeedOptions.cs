using System;
using CommandLine;

namespace Gnatta.Facebook.WebhookBackup.Commands
{
    // TODO ??
    [Verb("feed", HelpText = "Process feed.")]
    public class FeedOptions
    {
        [Value(0, Required = true, HelpText = "Token of the page to read conversations from.")]
        public string PageToken { get; set; }

        [Option("verbose", Default = false)]
        public bool Verbose { get; set; }

        [Option("start-time", Default = null, HelpText = "Time to start checking from, will send information for any conversation updated after this time. (Default: 15 minutes ago)")]
        public DateTime? StartTime { get; set; }

        [Option("end-time", Default = null, HelpText = "Time to stop checking, will stop polling at this time. (Default: 1 minute in the future, then the application will terminate)")]
        public DateTime? EndTime { get; set; }

        [Option("endpoint", HelpText = "Endpoint to send webhooks to. (Default: console output)")]
        public string WebhookEndpoint { get; set; }

        [Option("secret", Default = null, HelpText = "Facebook app secret of the app to sign requests. (Default: requests will not be signed)")]
        public string AppSecret { get; set; }

        [Option("poll-interval", Default = 30, HelpText = "Poll interval in seconds. (Default: 30)")]
        public int PollInterval { get; set; }

        [Option("fb-page-size", Default = 10, HelpText = "Page size during poll. (Default: 10)")]
        public int PageSize { get; set; }
    }
}
