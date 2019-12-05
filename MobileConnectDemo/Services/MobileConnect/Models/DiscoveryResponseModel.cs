using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class DiscoveryResponseModel
    {
        public string Ttl { get; set; }

        public DiscoveryResponseObject Response { get; set; }

        [JsonProperty("subscriber_id")]
        public string SubscriberId { get; set; }
    }

    public class DiscoveryResponseObject
    {
        [JsonProperty("serving_operator")]
        public string ServingOperator { get; set; }

        public DiscoveryResponseApis Apis { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }
    }

    public class DiscoveryResponseApis
    {
        [JsonProperty("operatorid")]
        public DiscoveryResponseOperatorId OperatorId { get; set; }
    }

    public class DiscoveryResponseOperatorId
    {
        public DiscoveryResponseOperatorId()
        {
            Links = new List<DiscoveryResponseLink>();
        }

        [JsonProperty("link")]
        public List<DiscoveryResponseLink> Links { get; set; }
    }

    public class DiscoveryResponseLink
    {
        public string Href { get; set; }

        public string Rel { get; set; }
    }
}