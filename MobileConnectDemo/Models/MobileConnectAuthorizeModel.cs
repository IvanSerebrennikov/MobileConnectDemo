using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.Models
{
    public class MobileConnectAuthorizeModel
    {
        public string PhoneNumber { get; set; }

        public string RedirectUrl { get; set; }

        public string NotificationUri { get; set; }

        public string DiscoveryUrl { get; set; }

        public string DiscoveryClientId { get; set; }

        public string DiscoveryPassword { get; set; }

        public string PrivateRsaKeyPath { get; set; }
    }
}