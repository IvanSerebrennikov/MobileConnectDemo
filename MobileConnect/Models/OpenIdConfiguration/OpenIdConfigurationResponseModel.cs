using Newtonsoft.Json;

namespace MobileConnect.Models.OpenIdConfiguration
{
    public class OpenIdConfigurationResponseModel
    {
        public string Issuer { get; set; }

        [JsonProperty("si_authorization_endpoint")]
        public string SiAuthorizationEndpoint { get; set; }
    }
}