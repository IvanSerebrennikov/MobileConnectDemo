using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MobileConnectDemo.Models
{
    public class MobileConnectNotifyModel
    {
        [JsonProperty("auth_req_id")]
        public string AuthReqId { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }
    }
}