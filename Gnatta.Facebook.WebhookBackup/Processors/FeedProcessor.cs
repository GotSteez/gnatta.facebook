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
    public class FeedProcessor : IProcessor
    {
        private const string FEED_ENDPOINT = "me/feed?fields=id,from,created_time,updated_time,message,type,status_type";

        private readonly ILog _log;
        private readonly FacebookClient _client;
        private readonly FacebookPageDetails _page;
        private readonly DateTime _start;
        private readonly DateTime _end;
        private readonly FeedOptions _options;
        
        // Stores the feed Id and the Updated Date
        private readonly Dictionary<string, DateTime> _seenFeed;

        public FeedProcessor(
            ILog log, 
            FacebookClient client, 
            FacebookPageDetails page, 
            DateTime start,
            DateTime end, 
            FeedOptions options)
        {
            _log = log;
            _client = client;
            _page = page;
            _start = start;
            _end = end;
            _options = options;
        }

        public void Process()
        {
            while (DateTime.UtcNow < _end)
            {
                ProcessPageFeed();
                _log.Info($"Process complete, sleeping for {_options.PollInterval}s");
                Thread.Sleep(TimeSpan.FromSeconds(_options.PollInterval));
            }
        }

        // needs a check in place to make sure we only get feed data from the designated start time 
        // create an enum of seen feed
        // use the updated date to figure out the verb
        // map based on the status_type
        // verb based on the updated_date and if we have seen it before 
        // type always appears to be status 
        
        private void ProcessPageFeed()
        {
            var cursors = new FacebookPagingCursors();
            var pageNumber = 0;

            while (cursors != null)
            {
                _log.Debug($"{FEED_ENDPOINT} {++pageNumber}");

                object parameters;
                if (string.IsNullOrWhiteSpace(cursors?.after))
                {
                    parameters = new {limit = _options.PageSize};
                }
                else
                {
                    parameters = new {limit = _options.PageSize, cursors?.after};
                }
                
                var current = _client.Get<FacebookFeedResult>(FEED_ENDPOINT, parameters);

                cursors = current.paging?.cursors;

                // Process page
                var updates = new List<FacebookPageFeedItem>();
                foreach (var feed in current.data)
                {
                    if (FeedHasntUpdated(feed))
                    {
                        cursors = null;
                        break;
                    }

                    // Process
                    _seenFeed[feed.id] = feed.updated_time;
                    updates.Add(feed);
                    _log.Debug(JsonConvert.SerializeObject(feed));
                }

                // Send webhook
                if (updates.Count > 0)
                {
                    var webhookModel = FacebookWebhookBuilder.BuildFromFeed(_page, updates);
                    var outboundProcessor = new OutboundWebhookProcessor(_log, _options.WebhookEndpoint, _options.AppSecret);
                    outboundProcessor.Send(webhookModel);
                }
           }
        }
        
        private bool FeedHasntUpdated(FacebookPageFeedItem feed)
        {
            return feed.updated_time < _start ||
                   _seenFeed.ContainsKey(feed.id) &&
                   _seenFeed[feed.id] == feed.updated_time;
        }
    }
}