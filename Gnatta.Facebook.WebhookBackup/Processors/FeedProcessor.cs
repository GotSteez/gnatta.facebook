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
        private const string FeedEndpoint = "me/feed?fields=id,from,created_time,updated_time,message,type,status_type";

        private readonly ILog _log;
        private readonly FacebookClient _client;
        private readonly FacebookPageDetails _page;
        private readonly DateTime _start;
        private readonly DateTime _end;
        private readonly Options _options;
        
        // Stores the feed Id and the Updated Date
        private readonly Dictionary<string, DateTime> _seenFeed;
        private readonly Dictionary<string, DateTime> _seenComments;

        public FeedProcessor(
            ILog log, 
            FacebookClient client, 
            FacebookPageDetails page, 
            DateTime start,
            DateTime end, 
            Options options)
        {
            _log = log;
            _client = client;
            _page = page;
            _start = start;
            _end = end;
            _options = options;
            
            _seenFeed = new Dictionary<string, DateTime>();
            _seenComments = new Dictionary<string, DateTime>();
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
        
        private void ProcessPageFeed()
        {
            var cursors = new FacebookPagingCursors();
            var pageNumber = 0;

            while (cursors != null)
            {
                _log.Debug($"{FeedEndpoint} {++pageNumber}");

                object parameters;
                if (string.IsNullOrWhiteSpace(cursors?.after))
                {
                    parameters = new {limit = _options.PageSize};
                }
                else
                {
                    parameters = new {limit = _options.PageSize, cursors?.after};
                }
                
                var current = _client.Get<FacebookFeedResult>(FeedEndpoint, parameters);

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

                    // Ignore comments for now.
//                    if (feed.updated_time > feed.created_time)
//                    {
//                        var comments = _client.Get($"{feed.id}/comments");
//                        Console.WriteLine("Comment found on" + feed.id);
//                        Console.WriteLine(JsonConvert.SerializeObject(comments));
//                    }
                    
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