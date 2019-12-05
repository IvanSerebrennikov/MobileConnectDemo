using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MobileConnectDemo.Models;
using MobileConnectDemo.Services.MobileConnect;
using MobileConnectDemo.Services.MobileConnect.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Controllers
{
    public class MobileConnectController : Controller
    {
        private readonly IMobileConnectService _mobileConnectService;

        public MobileConnectController()
        {
            _mobileConnectService = new MobileConnectService();
        }

        [HttpPost]
        public async Task<ActionResult> Authorize(MobileConnectAuthorizeModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber) ||
                string.IsNullOrEmpty(model.RedirectUrl) ||
                string.IsNullOrEmpty(model.DiscoveryUrl) ||
                string.IsNullOrEmpty(model.DiscoveryClientId) ||
                string.IsNullOrEmpty(model.DiscoveryPassword))
                return Content("Fill all fields");

            var correlationId = Guid.NewGuid().ToString();

            var discoveryRequestModel = new DiscoveryRequestModel
            {
                PhoneNumber = model.PhoneNumber,
                RedirectUrl = model.RedirectUrl,
                DiscoveryUrl = model.DiscoveryUrl,
                DiscoveryClientId = model.DiscoveryClientId,
                DiscoveryPassword = model.DiscoveryPassword,
                CorrelationId = correlationId
            };

            var discoveryResponse = await _mobileConnectService.SendDiscoveryRequest(discoveryRequestModel);

            if (discoveryResponse == null)
                return Content("Discovery Response is null");

            var openIdConfigurationRel = "openid-configuration";
            var openIdConfigurationUrl =
                discoveryResponse.Model?.Response?.Apis?.OperatorId?.Links.FirstOrDefault(x =>
                    x.Rel == openIdConfigurationRel)?.Href;

            if (string.IsNullOrEmpty(openIdConfigurationUrl))
                return Content("OpenId Configuration Url is null or empty");

            var openIdConfigurationResponse = await _mobileConnectService.SendOpenIdConfigurationRequest(openIdConfigurationUrl);

            return Content(
                JsonConvert.SerializeObject(new
                {
                    discoveryResponse = discoveryResponse,
                    openIdConfigurationResponse = openIdConfigurationResponse
                }));
        }
    }
}