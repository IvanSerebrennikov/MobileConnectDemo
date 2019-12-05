using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class DiscoveryRequestModel
    {
        public string PhoneNumber { get; set; }

        public string RedirectUrl { get; set; }

        public string DiscoveryUrl { get; set; }

        public string DiscoveryClientId { get; set; }

        public string DiscoveryPassword { get; set; }

        public string CorrelationId { get; set; }
    }
}