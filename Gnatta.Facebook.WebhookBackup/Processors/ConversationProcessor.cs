using System;
using System.Collections.Generic;
using System.Threading;
using Facebook;
using Gnatta.Facebook.Logging;
using Gnatta.Facebook.WebhookBackup.Commands;
using Gnatta.Facebook.WebhookBackup.Helpers;
using Gnatta.Facebook.WebhookBackup.Models;
using Newtonsoft.Json;

namespace Gnatta.Facebook.WebhookBackup.Processors
{
    public class ConversationProcessor : IProcessor
    {
        private const string CONVERSATIONS_ENDPOINT = "me/conversations";

        private readonly ILog _log;
        private readonly FacebookClient _client;
        private readonly FacebookPageDetails _page;
        private readonly DateTime _start;
        private readonly DateTime _end;
        private readonly Options _options;

        private readonly Dictionary<string, DateTime> _seenConversations;

        public ConversationProcessor(
            ILog log, FacebookClient client, FacebookPageDetails page, DateTime start, 
            DateTime end, Options options)
        {
            _log = log;
            _client = client;
            _page = page;
            _start = start;
            _end = end;
            _options = options;

            _seenConversations = new Dictionary<string, DateTime>();
        }

        public void Process()
        {
            while (DateTime.UtcNow < _end)
            {
                ProcessConversationPages();
                _log.Info($"Process complete, sleeping for {_options.PollInterval}s");
                Thread.Sleep(TimeSpan.FromSeconds(_options.PollInterval));
            }
        }

        private void ProcessConversationPages()
        {
            var cursors = new FacebookPagingCursors();
            var pageNumber = 0;

            while (cursors != null)
            {
                _log.Debug($"{CONVERSATIONS_ENDPOINT} {++pageNumber}");
                var current = _client.Get<FacebookConversationPage>(
                    CONVERSATIONS_ENDPOINT,
                    new { limit = _options.PageSize, cursors?.after });

                cursors = current.paging?.cursors;

                // Process page
                var updates = new List<FacebookConversationPageItem>();
                foreach (var conv in current.data)
                {
                    if (ConversationHasUpdated(conv))
                    {
                        cursors = null;
                        break;
                    }

                    // Process
                    _seenConversations[conv.id] = conv.updated_time;
                    updates.Add(conv);
                    _log.Debug(JsonConvert.SerializeObject(conv));
                }

                // Send webhook
                if (updates.Count > 0)
                {
                    var webhookModel = FacebookWebhookBuilder.BuildFromConversations(_page, updates);
                    var outboundProcessor = new OutboundWebhookProcessor(_log, _options.WebhookEndpoint, _options.AppSecret);
                    outboundProcessor.Send(webhookModel);
                }
            }
        }

        private bool ConversationHasUpdated(FacebookConversationPageItem conv)
        {
            return conv.updated_time < _start ||
                   _seenConversations.ContainsKey(conv.id) &&
                   _seenConversations[conv.id] == conv.updated_time;
        }
    }
}
