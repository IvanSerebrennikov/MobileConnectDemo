using MobileConnect.Interfaces;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;

namespace MobileConnect.Processors.SiAuthorize
{
    public class MobileConnectSiAuthorizeResult : IMobileConnectProcessResult
    {
        public DiscoveryResponse DiscoveryResponse { get; set; }

        public OpenIdConfigurationResponse OpenIdConfigurationResponse { get; set; }

        public SiAuthorizeResponse SiAuthorizeResponse { get; set; }

        public string ErrorMessage { get; set; }
    }
}