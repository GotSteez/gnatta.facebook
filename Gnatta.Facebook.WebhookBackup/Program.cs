using System;
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

        static int Main(string[] args)
        {
            return Parser.Default
                .ParseArguments<ConversationOptions, FeedOptions>(args)
                .MapResult<ConversationOptions, FeedOptions, int>(RunConversation, RunFeed, errs => 1);
        }

        private static int RunFeed(FeedOptions options)
        {
            _log = LogFactory.Build();
            _log.Info("Processing Feed");
            // Validate options
            DateTime start, end;
            try
            {
                start = options.StartTime ?? DateTime.UtcNow.AddMinutes(-15);
                end = options.EndTime ?? DateTime.UtcNow.AddMinutes(1);
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

        private static int RunConversation(ConversationOptions options)
        {
            _log = LogFactory.Build(options.Verbose);
            _log.Info("Processing Conversations");

            // Validate options
            DateTime start, end;
            try
            {
                start = options.StartTime ?? DateTime.UtcNow.AddMinutes(-15);
                end = options.EndTime ?? DateTime.UtcNow.AddMinutes(1);
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
    }
}
