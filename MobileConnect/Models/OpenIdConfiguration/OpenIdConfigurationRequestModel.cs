using MobileConnect.Models.Base;

namespace MobileConnect.Models.OpenIdConfiguration
{
    public class OpenIdConfigurationRequestModel : BaseMobileConnectRequestModel
    {
        public string OpenIdConfigurationUrl { get; set; }
    }
}