using System;
using System.Threading.Tasks;
using CommandLine;
using Facebook;
using Gnatta.Facebook.Logging;
using Gnatta.Facebook.WebhookBackup.Commands;
using Gnatta.Facebook.WebhookBackup.Models;
using Gnatta.Facebook.WebhookBackup.Processors;

namespace Gnatta.Facebook.WebhookBackup
{
    class Program
    {
        private static ILog _log;

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => Run(opts).Wait());
        }

        private static async Task Run(Options opts)
        {
            _log = LogFactory.Build(opts.Verbose);

            Task convTask = null;
            Task feedTask = null;
            
            if (opts.Conv)
            {
                convTask = Task.Run(() => RunConversation(opts));
            }

            if (opts.Feed)
            {
                feedTask = Task.Run(() => RunFeed(opts));
            }

            if (convTask != null)
            {
                await convTask;
            }

            if (feedTask != null)
            {
                await feedTask;
            }
        }

        private static int RunFeed(Options options)
        {   
            _log.Info("Processing Feed");
            
            // Validate options
            DateTime start, end;
            try
            {
                start = GetStart(options.StartTime);
                end = GetEnd(options.EndTime);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return 1;
            }
            
            // Initialise connection
            var client = new FacebookClient(options.PageToken) { Version = "v2.10" };
            FacebookPageDetails page;
            try
            {
                page = client.Get<FacebookPageDetails>("me");
                _log.Info($"Connected: {page.id} ({page.name})");
            }
            catch (FacebookOAuthException fbex)
            {
                _log.Error($"Cannot connect: {fbex.Message}");
                return 1;
            }
            catch (Exception e)
            {
                _log.Error($"Something went wrong! {e.Message}");
                return 1;
            }

            // Process
            try
            {
                var processor = new FeedProcessor(_log, client, page, start, end, options);
                processor.Process();
            }
            catch (Exception e)
            {
                _log.Error("Error during processing", e);
                return 1;
            }

            return 0;
        }

        private static int RunConversation(Options options)
        {
            _log.Info("Processing Conversations");

            // Validate options
            DateTime start, end;
            try
            {
                start = GetStart(options.StartTime);
                end = GetEnd(options.EndTime);
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return 1;
            }

            // Initialise connection
            var client = new FacebookClient(options.PageToken) { Version = "v2.10" };
            FacebookPageDetails page;
            try
            {
                page = client.Get<FacebookPageDetails>("me");
                _log.Info($"Connected: {page.id} ({page.name})");
            }
            catch (FacebookOAuthException fbex)
            {
                _log.Error($"Cannot connect: {fbex.Message}");
                return 1;
            }
            catch (Exception e)
            {
                _log.Error($"Something went wrong! {e.Message}");
                return 1;
            }

            // Process
            try
            {
                var processor = new ConversationProcessor(_log, client, page, start, end, options);
                processor.Process();
            }
            catch (Exception e)
            {
                _log.Error("Error during processing", e);
                return 1;
            }

            return 0;
        }

        private static DateTime GetStart(DateTime? start)
        {
            return start ?? DateTime.UtcNow.AddMinutes(-15);
        }

        private static DateTime GetEnd(DateTime? end)
        {
            return end ?? DateTime.UtcNow.AddMinutes(1);
        }
    }
}
