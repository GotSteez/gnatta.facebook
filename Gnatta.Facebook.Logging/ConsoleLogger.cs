using System;

namespace Gnatta.Facebook.Logging
{
    public class ConsoleLogger : ILog
    {
        private readonly bool _verbose;

        public ConsoleLogger(bool verbose = true)
        {
            _verbose = verbose;
        }

        private void Log(string level, string text)
        {
            Console.WriteLine($"{DateTime.Now:O} {level} {text}");
        }

        public void Debug(object log)
        {
            if (!_verbose) return;
            Log("DEBUG", log.ToString());
        }

        public void Info(object log)
        {
            Log("INFO", log.ToString());
        }

        public void Error(object log, Exception ex = null)
        {
            var msg = log.ToString();
            if (ex != null)
            {
                msg += $" {ex.Message}\r\n{ex.StackTrace}";
            }

            Log("ERROR", msg);
        }
    }
}
