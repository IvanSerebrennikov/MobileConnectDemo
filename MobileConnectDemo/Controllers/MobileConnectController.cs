using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MobileConnect.Interfaces;
using MobileConnect.Processors.SiAuthorize;
using MobileConnect.Services;
using MobileConnectDemo.DataAccess;
using MobileConnectDemo.DataAccess.Entities;
using MobileConnectDemo.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Controllers
{
    public class MobileConnectController : Controller
    {
        private readonly IMobileConnectService _mobileConnectService;

        private readonly Repository _repository;

        public MobileConnectController()
        {
            // TODO: Get from ctor DI
            _mobileConnectService = new MobileConnectService();
            _repository = new Repository();
        }

        [HttpPost]
        public async Task<ActionResult> Authorize(MobileConnectAuthorizeModel model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber) ||
                string.IsNullOrEmpty(model.RedirectUrl) ||
                string.IsNullOrEmpty(model.NotificationUri) ||
                string.IsNullOrEmpty(model.DiscoveryUrl) ||
                string.IsNullOrEmpty(model.DiscoveryClientId) ||
                string.IsNullOrEmpty(model.DiscoveryPassword) ||
                string.IsNullOrEmpty(model.PrivateRsaKeyPath))
                return Content("Fill all fields");

            var authorizeSettings = new MobileConnectSiAuthorizeSettings
            {
                PhoneNumber = model.PhoneNumber,
                RedirectUrl = model.RedirectUrl,
                NotificationUri = model.NotificationUri,
                DiscoveryUrl = model.DiscoveryUrl,
                DiscoveryClientId = model.DiscoveryClientId,
                DiscoveryPassword = model.DiscoveryPassword,
                PrivateRsaKeyPath = model.PrivateRsaKeyPath
            };

            var authorizeResult = await _mobileConnectService.SiAuthorize(authorizeSettings);

            if (!authorizeResult.IsSucceeded)
            {
                // TODO: Log authorizeResult.ToString()
            }

            var mobileConnectRequest = new MobileConnectAuthorizeRequest
            {
                PhoneNumber = model.PhoneNumber,
                ClientNotificationToken = authorizeResult.ClientNotificationToken,
                AuthReqId = authorizeResult.AuthReqId,
                CorrelationId = authorizeResult.CorrelationId,
                Nonce = authorizeResult.Nonce,
                IsResponseSucceeded = authorizeResult.IsSucceeded,
                ResponseErrorMessage = authorizeResult.ErrorMessage
            };

            _repository.CreateMobileConnectAuthorizeRequest(mobileConnectRequest);

            return PartialView("_MobileConnectAuthorizePartial", authorizeResult);
        }
    }
}