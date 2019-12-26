using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MobileConnect.Interfaces;
using MobileConnect.Processors.SiAuthorize;
using MobileConnect.Services;
using MobileConnectDemo.DataAccess;
using MobileConnectDemo.DataAccess.Entities;
using MobileConnectDemo.Logger;
using MobileConnectDemo.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Controllers
{
    public class MobileConnectController : Controller
    {
        private static readonly FakeLogger MobileConnectAuthorizeLogger = new FakeLogger();
        private static readonly FakeLogger MobileConnectNotifyLogger = new FakeLogger();

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
                MobileConnectAuthorizeLogger.Warn(authorizeResult.ToString());
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

        [HttpPost]
        public ActionResult Notify()
        {
            var notifyGuid = Guid.NewGuid().ToString();

            var notifyModel = ValidateNotifyRequestAndGetMobileConnectNotifyModel(notifyGuid, out var validationErrorMessage);

            if (!string.IsNullOrEmpty(validationErrorMessage))
            {
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid}]. {validationErrorMessage}");

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, validationErrorMessage);
            }

            var mobileConnectRequest =
                _repository.GetMobileConnectAuthorizeRequest(notifyModel.AuthReqId, notifyModel.CorrelationId);

            if (mobileConnectRequest == null)
            {
                var errorMessage = "mobile connect request not found";
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid}]. {errorMessage}");

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, errorMessage);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private MobileConnectNotifyModel ValidateNotifyRequestAndGetMobileConnectNotifyModel(
            string notifyGuid, out string validationErrorMessage)
        {
            MobileConnectNotifyModel notifyModel;

            using (var requestBodyStream = Request.InputStream)
            {
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                var json = new StreamReader(requestBodyStream).ReadToEnd();

                MobileConnectNotifyLogger.Info(
                    $"Notify [{notifyGuid}]. " +
                    $"Body: {json}; " +
                    $"Headers: {string.Join(", ", Request.Headers.AllKeys.Select(x => $"{x}: {Request.Headers[x]}").ToList())}");

                notifyModel = JsonConvert.DeserializeObject<MobileConnectNotifyModel>(json);
            }

            if (notifyModel == null)
            {
                validationErrorMessage = "notify model is null";
                return null;
            }

            if (string.IsNullOrEmpty(notifyModel.AuthReqId))
            {
                validationErrorMessage = "auth_req_id is null or empty";
                return null;
            }

            if (string.IsNullOrEmpty(notifyModel.CorrelationId))
            {
                validationErrorMessage = "correlation_id is null or empty";
                return null;
            }

            validationErrorMessage = "";
            return notifyModel;
        }
    }
}