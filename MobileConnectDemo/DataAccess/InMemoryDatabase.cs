using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MobileConnectDemo.DataAccess.Entities;

namespace MobileConnectDemo.DataAccess
{
    public static class InMemoryDatabase
    {
        static InMemoryDatabase()
        {
            MobileConnectAuthorizeRequests = new HashSet<MobileConnectAuthorizeRequest>();
        }

        public static HashSet<MobileConnectAuthorizeRequest> MobileConnectAuthorizeRequests { get; set; }
    }
}