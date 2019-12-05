using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class OpenIdConfigurationResponse
    {
        public OpenIdConfigurationRequestModel Model { get; set; }

        public string JsonString { get; set; }
    }
}