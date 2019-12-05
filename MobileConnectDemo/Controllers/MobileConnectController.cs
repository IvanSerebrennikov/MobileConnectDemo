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

            var authorizeSettings = new MobileConnectAuthorizeSettings
            {
                PhoneNumber = model.PhoneNumber,
                RedirectUrl = model.RedirectUrl,
                DiscoveryUrl = model.DiscoveryUrl,
                DiscoveryClientId = model.DiscoveryClientId,
                DiscoveryPassword = model.DiscoveryPassword
            };

            var authorizeResult = await _mobileConnectService.SiAuthorize(authorizeSettings);

            return PartialView("_MobileConnectAuthorizePartial", authorizeResult);
        }
    }
}