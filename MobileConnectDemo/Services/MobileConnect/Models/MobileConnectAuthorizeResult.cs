using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class MobileConnectAuthorizeResult
    {
        public DiscoveryResponse DiscoveryResponse { get; set; }

        public OpenIdConfigurationResponse OpenIdConfigurationResponse { get; set; }

        public string ErrorMessage { get; set; }
    }
}