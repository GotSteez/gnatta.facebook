using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Gnatta.Facebook.Logging;
using Gnatta.Facebook.WebhookBackup.Models;
using Newtonsoft.Json;

namespace Gnatta.Facebook.WebhookBackup.Processors
{
    public class OutboundWebhookProcessor
    {
        private const string USER_AGENT = "Gnatta.Facebook";
        private const string FB_SIGNATURE_HEADER = "X-Hub-Signature";

        private readonly ILog _log;
        private readonly string _endpoint;
        private readonly string _appSecret;

        public OutboundWebhookProcessor(ILog log, string endpoint, string appSecret)
        {
            _log = log;
            _endpoint = endpoint;
            _appSecret = appSecret;
        }

        public void Send(FacebookWebhook model)
        {
            var body = JsonConvert.SerializeObject(model);

            // default to console if there is no endpoint
            if (string.IsNullOrWhiteSpace(_endpoint))
            {
                _log.Info(body);
                return;
            }
            
            var req = (HttpWebRequest)WebRequest.Create(_endpoint);
            req.Method = "POST";
            req.UserAgent = USER_AGENT;
            req.ContentType = "application/json";

            if (!string.IsNullOrWhiteSpace(_appSecret))
            {
                req.Headers.Add(FB_SIGNATURE_HEADER, GenerateSignature(body));
            }

            using (var reqStream = req.GetRequestStream())
            using (var reqWriter = new StreamWriter(reqStream))
            {
                reqWriter.Write(body);
                reqWriter.Flush();
            }

            using (var res = (HttpWebResponse)req.GetResponse())
            {
                _log.Debug($"{_endpoint} {(int)res.StatusCode} {res.StatusDescription}");
            }
        }

        private string GenerateSignature(string body)
        {
            using (var sha = new HMACSHA1(Encoding.UTF8.GetBytes(_appSecret)))
            {
                var generated = sha.ComputeHash(Encoding.UTF8.GetBytes(body));
                return "sha1=" + string.Join("", generated.Select(x => x.ToString("x2")));
            }
        }
    }
}
