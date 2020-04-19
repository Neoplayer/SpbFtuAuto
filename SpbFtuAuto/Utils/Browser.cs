using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SpbFtuAuto.Utils
{
    /// <summary>
    /// Browser class.
    /// <typeparam name="T">enum of request type</typeparam>
    /// </summary>
    internal class Browser<T> : IDisposable
    {
        public Browser()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true, // This must be false if we want to handle custom redirection schemes such as "steammobile"
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                CookieContainer = CookieContainer,
                MaxConnectionsPerServer = MaxConnections,
                UseProxy = false
            };
            HttpClient = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(60) };
            HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Sutox/0.1");
        }

        public Dictionary<T, RequestDelay> Delays;


        private List<Request<T>> RequestsHistory = new List<Request<T>>();

        internal const byte MaxTries = 5;
        private const byte MaxConnections = 10; // Defines maximum number of connections per ServicePoint. Be careful, as it also defines maximum number of sockets in CLOSE_WAIT state
        private const byte MaxIdleTime = 15; // Defines in seconds, how long socket is allowed to stay in CLOSE_WAIT state after there are no connections to it
        internal readonly CookieContainer CookieContainer = new CookieContainer();
        private readonly HttpClient HttpClient;
        public void Dispose() => HttpClient.Dispose();

        
        /// <summary>
        /// Method that executes the request and logs it in the browser history
        /// </summary>
        /// <param name="request">Request with the request type T</param>
        /// <typeparam name="T">enum type of request</typeparam>
        /// <returns>Modified object with Task<HttpResponseMessage></returns>
        internal Request<T> ExecuteRequest(Request<T> request)
        {
            HttpRequestMessage requestMsg = new HttpRequestMessage(request.RequestMethod, request.RequestUrl);
            if (request.RequestContent != null && request.RequestMethod == HttpMethod.Post)    // Maybe exception if post request without content?  
            {
                try
                {
                    requestMsg.Content = new FormUrlEncodedContent(request.RequestContent);
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }
            if (!string.IsNullOrEmpty(request.Referer))
            {
                requestMsg.Headers.Referrer = new Uri(request.Referer);
            }

            request.HttpRespMsg = HttpClient.SendAsync(requestMsg, HttpCompletionOption.ResponseContentRead);
            // Logger.Log(" Just a request to steam passing by...");
            RequestsHistory.Add(request);

            return request;
        }

        private bool DelayCheck(T request)
        {
            int S = RequestsHistory.Count(x => x.Time > DateTime.Now + TimeSpan.FromSeconds(1));
            int M = RequestsHistory.Count(x => x.Time > DateTime.Now + TimeSpan.FromMinutes(1));
            int H = RequestsHistory.Count(x => x.Time > DateTime.Now + TimeSpan.FromHours(1));
            return Delays[request].S < S && Delays[request].M < M && Delays[request].H < H;
        }

        public bool InternetConnectionCheck(int timeout = 1000)
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    internal class RequestDelay
    {
        public uint S { get; set; }
        public uint M { get; set; }
        public uint H { get; set; }
    }

    internal class Request<T>
    {
        public Request(HttpMethod requestMethod, string requestUrl, T requestType, string referer = null)
        {
            RequestMethod = requestMethod;
            RequestUrl = requestUrl;
            RequestType = requestType;
            Referer = referer;
            Time = DateTime.Now;
        }

        public Request(HttpMethod requestMethod, string requestUrl, T requestType, IReadOnlyCollection<KeyValuePair<string, string>> requestContent, string referer = null)
        {
            RequestMethod = requestMethod;
            RequestUrl = requestUrl;
            RequestType = requestType;
            RequestContent = requestContent;
            Referer = referer;
            Time = DateTime.Now;
        }

        public HttpMethod RequestMethod { get; set; }
        public string RequestUrl { get; set; }
        public T RequestType { get; set; }
        public IReadOnlyCollection<KeyValuePair<string, string>> RequestContent { get; set; }
        public string Referer { get; set; }
        public DateTime Time { get; set; }
        public Task<HttpResponseMessage> HttpRespMsg { get; set; }
        public string GetContent()
        {
            try
            {
                return HttpRespMsg.Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Logger.Log(e.StackTrace, Logger.LogType.Red);
                return null;
            }
        }
    }


    internal enum ESteamRequestType
    {
        Request
    }
}
