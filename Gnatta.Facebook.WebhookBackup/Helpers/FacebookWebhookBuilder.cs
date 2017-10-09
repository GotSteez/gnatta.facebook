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
                        }).ToArray()
                    }
                }
            };
        }
    }
}
