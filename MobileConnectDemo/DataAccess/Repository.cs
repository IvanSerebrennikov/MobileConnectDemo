using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MobileConnectDemo.DataAccess.Entities;

namespace MobileConnectDemo.DataAccess
{
    public class Repository
    {
        public void CreateMobileConnectAuthorizeRequest(MobileConnectAuthorizeRequest mobileConnectRequest)
        {
            InMemoryDatabase.MobileConnectAuthorizeRequests.Add(mobileConnectRequest);
        }
    }
}