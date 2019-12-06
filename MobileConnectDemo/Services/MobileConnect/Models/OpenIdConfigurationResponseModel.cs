using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class OpenIdConfigurationResponseModel
    {
        public string Issuer { get; set; }

        [JsonProperty("si_authorization_endpoint")]
        public string SiAuthorizationEndpoint { get; set; }
    }
}