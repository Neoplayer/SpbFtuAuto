using System.Net;
using Newtonsoft.Json;

namespace SpbFtuAuto.Data.Requests
{
    public class ResponseBase
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}