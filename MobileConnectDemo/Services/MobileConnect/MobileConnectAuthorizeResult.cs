using MobileConnectDemo.Services.MobileConnect.Models;

namespace MobileConnectDemo.Services.MobileConnect
{
    public class MobileConnectAuthorizeResult
    {
        public DiscoveryResponse DiscoveryResponse { get; set; }

        public OpenIdConfigurationResponse OpenIdConfigurationResponse { get; set; }

        public SiAuthorizeResponse SiAuthorizeResponse { get; set; }

        public string ErrorMessage { get; set; }
    }
}