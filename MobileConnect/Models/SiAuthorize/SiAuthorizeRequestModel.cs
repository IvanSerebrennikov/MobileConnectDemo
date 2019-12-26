using Newtonsoft.Json;

namespace MobileConnect.Models.SiAuthorize
{
    public class SiAuthorizeRequestModel
    {
        public string SiAuthorizationUrl { get; set; }

        public string PrivateRsaKeyPath { get; set; }

        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public SiAuthorizeRequestObjectClaims RequestObjectClaims { get; set; }
    }

    public class SiAuthorizeRequestObjectClaims
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("login_hint")]
        public string LoginHint { get; set; }

        [JsonProperty("acr_values")]
        public string AcrValues { get; set; }

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
    }
}