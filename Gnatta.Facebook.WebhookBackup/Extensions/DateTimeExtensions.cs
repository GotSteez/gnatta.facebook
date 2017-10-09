using System;

namespace Gnatta.Facebook.WebhookBackup.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnix(this DateTime datetime)
        {
            return (long)datetime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
