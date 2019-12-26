using System.Collections.Generic;
using MobileConnect.Interfaces;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobileConnect.Processors.SiAuthorize
{
    public class MobileConnectSiAuthorizeResult : IMobileConnectProcessResult
    {
        public string ClientNotificationToken { get; set; }

        public string AuthReqId { get; set; }

        public string CorrelationId { get; set; }

        public string Nonce { get; set; }

        public DiscoveryResponse DiscoveryResponse { get; set; }

        public OpenIdConfigurationResponse OpenIdConfigurationResponse { get; set; }

        public SiAuthorizeResponse SiAuthorizeResponse { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsSucceeded => string.IsNullOrEmpty(ErrorMessage);

        public override string ToString()
        {
            var properties = new Dictionary<string, object>
            {
                {"ClientNotificationToken", ClientNotificationToken},
                {"AuthReqId", AuthReqId},
                {"CorrelationId", CorrelationId},
                {"Nonce", Nonce},
                {"ErrorMessage", ErrorMessage},
                {
                    "DiscoveryResponse",
                    !string.IsNullOrEmpty(DiscoveryResponse?.JsonString)
                        ? JToken.Parse(DiscoveryResponse.JsonString)
                        : null
                },
                {
                    "OpenIdConfigurationResponse",
                    !string.IsNullOrEmpty(OpenIdConfigurationResponse?.JsonString)
                        ? JToken.Parse(OpenIdConfigurationResponse.JsonString)
                        : null
                },
                {
                    "SiAuthorizeResponse",
                    !string.IsNullOrEmpty(SiAuthorizeResponse?.JsonString)
                        ? JToken.Parse(SiAuthorizeResponse.JsonString)
                        : null
                }
            };

            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }
    }
}