using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileConnectDemo.Services.MobileConnect.Models;

namespace MobileConnectDemo.Services.MobileConnect
{
    public interface IMobileConnectService
    {
        Task<DiscoveryResponse> SendDiscoveryRequest(DiscoveryRequestModel requestModel);
    }
}
