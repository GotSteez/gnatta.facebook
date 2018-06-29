using System;
using System.Collections.Generic;
using System.Linq;
using Gnatta.Facebook.WebhookBackup.Extensions;
using Gnatta.Facebook.WebhookBackup.Models;

namespace Gnatta.Facebook.WebhookBackup.Helpers
{
    public static class FacebookWebhookBuilder
    {
        public static FacebookWebhook BuildFromConversations(FacebookPageDetails page, IList<FacebookConversationPageItem> conversations)
        {
            return new FacebookWebhook
            {
                entry = new [] {
                    new FacebookWebhookEntry
                    {
                        id = page.id,
                        time = DateTime.UtcNow.ToUnix(),
                        changes = conversations.Select(conv => new FacebookWebhookConversationChange
                        {
                            value = new FacebookWebhookConversationValue
                            {
                                thread_id = conv.id,
                                thread_key = conv.id,
                                page_id = long.Parse(page.id)
                            }
                        })
                    }
                }
            };
        }

        public static FacebookWebhook BuildFromFeed(FacebookPageDetails page, IList<FacebookPageFeedItem> feeds)
        {
            return new FacebookWebhook
            {
                entry = new []
                {
                    new FacebookWebhookEntry
                    {
                        id = page.id,
                        time = DateTime.UtcNow.ToUnix(),
                        changes = feeds.Select(feed => new FacebookWebhookFeedChange
                        {
                            value = new FacebookWebhookFeedValue
                            {
                                item = MapFeedItem(feed.status_type),
                                created_time = feed.created_time.ToUnix(),
                                from = feed.from,
                                message = feed.message,
                                post_id = feed.id,
                                published = 1,
                                verb = MapFeedVerb(feed)
                            }
                        })
                    }
                }
            };
        }

        private static string MapFeedItem(string statusType)
        {
            switch (statusType)
            {
                case "mobile_status_update":
                    return "status";
                case "wall_post":
                    return "post";
                default:
                    throw new Exception($"Unknown status_type field provided {statusType}");
            }
        }

        private static string MapFeedVerb(FacebookPageFeedItem feed)
        {
            return $"{MapFeedItem(feed.status_type)}:add";
        }
    }
}
