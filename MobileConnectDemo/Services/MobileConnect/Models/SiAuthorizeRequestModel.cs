using System.Collections.Generic;
using MobileConnectDemo.Helpers;
using Newtonsoft.Json;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class SiAuthorizeRequestModel
    {
        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public SiAuthorizeRequestObjectClaims RequestObjectClaims { get; set; }
    }

    public class SiAuthorizeRequestObjectClaims
    {
        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("login_hint")]
        public string LoginHint { get; set; }

        [JsonProperty("acr_values")]
        public string ArcValues { get; set; }

        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }

        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("aud")]
        public string Aud { get; set; }

        [JsonProperty("client_notification_token")]
        public string ClientNotificationToken { get; set; }

        [JsonProperty("notification_uri")]
        public string NotificationUri { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        public string ToJwtToken(string privateRsaKey)
        {
            var json = JsonConvert.SerializeObject(this);
            var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            return payload.ToJwtTokenWithRs256(privateRsaKey);
        }
    }
}