using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MobileConnect.Helpers;
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

                return BadRequestResult(validationErrorMessage);
            }

            var mobileConnectRequest =
                _repository.GetMobileConnectAuthorizeRequest(notifyModel.AuthReqId, notifyModel.CorrelationId);

            if (mobileConnectRequest == null)
            {
                var errorMessage = "mobile connect request not found";
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid}]. {errorMessage}");

                return BadRequestResult(errorMessage);
            }

            mobileConnectRequest.IsNotificationReceived = true;

            ValidateNotifyRequestAuthorization(mobileConnectRequest.ClientNotificationToken, out var authErrorMessage);

            if (!string.IsNullOrEmpty(authErrorMessage))
            {
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid} {mobileConnectRequest.Id}]. {authErrorMessage}");

                return BadRequestResult(authErrorMessage);
            }

            var idTokenClaims = notifyModel.IdToken.GetJwtTokenClaims();
            var nonce = idTokenClaims.FirstOrDefault(x => x.Type == "nonce");
            if (nonce?.Value != mobileConnectRequest.Nonce)
            {
                var errorMessage = "id_token nonce is null or invalid";
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid} {mobileConnectRequest.Id}]. {errorMessage}");

                return BadRequestResult(errorMessage);
            }

            var accessTokenClaims = notifyModel.AccessToken.GetJwtTokenClaims();
            var sub = accessTokenClaims.FirstOrDefault(x => x.Type == "sub");
            if (sub?.Value != mobileConnectRequest.PhoneNumber)
            {
                var errorMessage = "access_token sub is null or invalid";
                MobileConnectNotifyLogger.Warn(
                    $"Notify [{notifyGuid} {mobileConnectRequest.Id}]. {errorMessage}");

                return BadRequestResult(errorMessage);
            }

            mobileConnectRequest.IsAuthorized = true;

            MobileConnectNotifyLogger.Info(
                $"Notify [{notifyGuid} {mobileConnectRequest.Id}]. Authorized.");

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

        private void ValidateNotifyRequestAuthorization(
            string clientNotificationToken, out string authErrorMessage)
        {
            var authHeader = Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                authErrorMessage = "Authorization header is null or empty";
                return;
            }

            var authHeaderParts = authHeader.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (authHeaderParts.Length != 2)
            {
                authErrorMessage = "Authorization header is incorrect";
                return;
            }

            if (authHeaderParts[0] != "Bearer")
            {
                authErrorMessage = "Authorization header scheme is incorrect";
                return;
            }

            if (authHeaderParts[1] != clientNotificationToken)
            {
                authErrorMessage = "Authorization header value is incorrect";
                return;
            }

            authErrorMessage = "";
        }

        private ActionResult BadRequestResult(string message)
        {
            Response.StatusCode = 400; 

            return Json(new { message }, JsonRequestBehavior.AllowGet);
        }
    }
}