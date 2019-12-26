using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;
using MobileConnect.Processors.Base;

namespace MobileConnect.Processors.SiAuthorize
{
    public class MobileConnectSiAuthorizeProcessor : MobileConnectProcessor<MobileConnectSiAuthorizeResult,
        MobileConnectSiAuthorizeSettings>
    {
        private readonly string _correlationId;

        public MobileConnectSiAuthorizeProcessor()
        {
            _correlationId = Guid.NewGuid().ToString();

            Result.CorrelationId = _correlationId;
        }

        protected override async Task Process()
        {
            var (openIdConfigurationUrl, clientId) = await ProcessDiscovery();

            var (audience, siAuthorizationEndpoint) = await ProcessOpenIdConfiguration(openIdConfigurationUrl);

            await ProcessSiAuthorize(clientId, audience, siAuthorizationEndpoint);
        }

        private async Task<(string openIdConfigurationUrl, string clientId)> ProcessDiscovery()
        {
            var emptyResult = ("", "");

            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return emptyResult;

            if (string.IsNullOrEmpty(_correlationId))
                return emptyResult;

            var discoveryRequest = new DiscoveryRequestModel
            {
                PhoneNumber = Settings.PhoneNumber,
                RedirectUrl = Settings.RedirectUrl,
                DiscoveryUrl = Settings.DiscoveryUrl,
                DiscoveryClientId = Settings.DiscoveryClientId,
                DiscoveryPassword = Settings.DiscoveryPassword,
                CorrelationId = _correlationId
            };

            var discoveryResponse = await Client.SendDiscoveryRequest(discoveryRequest);
            if (discoveryResponse == null)
            {
                Result.ErrorMessage = "Discovery Response is null";
                return emptyResult;
            }

            Result.DiscoveryResponse = discoveryResponse;

            if (!discoveryResponse.IsSucceeded)
            {
                Result.ErrorMessage = "Discovery Response StatusCode is not success";
                return emptyResult;
            }

            if (!TryGetOpenIdConfigurationUrl(discoveryResponse, out var openIdConfigurationUrl))
                return emptyResult;

            if (!TryGetClientId(discoveryResponse, out var clientId))
                return emptyResult;

            return (openIdConfigurationUrl, clientId);
        }

        private async Task<(string audience, string siAuthorizationEndpoint)> ProcessOpenIdConfiguration(
            string openIdConfigurationUrl)
        {
            var emptyResult = ("", "");

            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return emptyResult;

            if (string.IsNullOrEmpty(_correlationId) ||
                string.IsNullOrEmpty(openIdConfigurationUrl))
                return emptyResult;

            var openIdConfigurationRequestModel = new OpenIdConfigurationRequestModel
            {
                OpenIdConfigurationUrl = openIdConfigurationUrl
            };

            var openIdConfigurationResponse =
                await Client.SendOpenIdConfigurationRequest(openIdConfigurationRequestModel);
            if (openIdConfigurationResponse == null)
            {
                Result.ErrorMessage = "OpenId Configuration Response is null";
                return emptyResult;
            }

            Result.OpenIdConfigurationResponse = openIdConfigurationResponse;

            if (!openIdConfigurationResponse.IsSucceeded)
            {
                Result.ErrorMessage = "OpenId Configuration Response StatusCode is not success";
                return emptyResult;
            }

            if (!TryGetAudience(openIdConfigurationResponse, out var audience))
                return emptyResult;

            if (!TryGetSiAuthorizationEndpoint(openIdConfigurationResponse, out var siAuthorizationEndpoint))
                return emptyResult;

            return (audience, siAuthorizationEndpoint);
        }

        private async Task ProcessSiAuthorize(string clientId, string audience, string siAuthorizationEndpoint)
        {
            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(_correlationId) ||
                string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(audience) ||
                string.IsNullOrEmpty(siAuthorizationEndpoint))
                return;

            var clientNotificationToken = Guid.NewGuid().ToString();
            var nonce = Guid.NewGuid().ToString();

            Result.ClientNotificationToken = clientNotificationToken;
            Result.Nonce = nonce;

            var responseType = "mc_si_async_code";
            var scope = "openid mc_authn";
            var acrValues = "3 2";
            var loginHint = WebUtility.UrlEncode($"MSISDN:{Settings.PhoneNumber}");
            var version = "mc_si_r2_v1.0";

            var siAuthorizeRequestModel = new SiAuthorizeRequestModel
            {
                SiAuthorizationUrl = siAuthorizationEndpoint,
                PrivateRsaKeyPath = Settings.PrivateRsaKeyPath,
                ResponseType = responseType,
                ClientId = clientId,
                Scope = scope,
                RequestObjectClaims = new SiAuthorizeRequestObjectClaims
                {
                    ResponseType = responseType,
                    ClientId = clientId,
                    Scope = scope,
                    Nonce = nonce,
                    LoginHint = loginHint,
                    AcrValues = acrValues,
                    CorrelationId = _correlationId,
                    Iss = clientId,
                    Aud = audience,
                    ClientNotificationToken = clientNotificationToken,
                    NotificationUri = Settings.NotificationUri,
                    Version = version
                }
            };

            var siAuthorizeResponse =
                await Client.SendSiAuthorizeRequest(siAuthorizeRequestModel);
            if (siAuthorizeResponse == null)
            {
                Result.ErrorMessage = "SI Authorize Response is null";
                return;
            }

            Result.SiAuthorizeResponse = siAuthorizeResponse;

            if (!siAuthorizeResponse.IsSucceeded)
            {
                Result.ErrorMessage = "SI Authorize Response StatusCode is not success";
                return;
            }

            if (!TryGetAuthReqId(siAuthorizeResponse, out var authReqId))
                return;

            Result.AuthReqId = authReqId;
        }

        private bool TryGetOpenIdConfigurationUrl(DiscoveryResponse discoveryResponse,
            out string openIdConfigurationUrl)
        {
            var openIdConfigurationRel = "openid-configuration";

            openIdConfigurationUrl =
                discoveryResponse.Model?.Response?.Apis?.OperatorId?.Links.FirstOrDefault(x =>
                    x.Rel == openIdConfigurationRel)?.Href;

            if (string.IsNullOrEmpty(openIdConfigurationUrl))
            {
                Result.ErrorMessage = "OpenId Configuration Url is null or empty";
                return false;
            }

            return true;
        }

        private bool TryGetClientId(DiscoveryResponse discoveryResponse, 
            out string clientId)
        {
            clientId =
                discoveryResponse.Model?.Response?.ClientId;

            if (string.IsNullOrEmpty(clientId))
            {
                Result.ErrorMessage = "ClientId is null or empty";
                return false;
            }

            return true;
        }

        private bool TryGetAudience(OpenIdConfigurationResponse openIdConfigurationResponse, 
            out string audience)
        {
            audience =
                openIdConfigurationResponse.Model?.Issuer;

            if (string.IsNullOrEmpty(audience))
            {
                Result.ErrorMessage = "Audience is null or empty";
                return false;
            }

            return true;
        }

        private bool TryGetSiAuthorizationEndpoint(OpenIdConfigurationResponse openIdConfigurationResponse,
            out string siAuthorizationEndpoint)
        {
            siAuthorizationEndpoint =
                openIdConfigurationResponse.Model?.SiAuthorizationEndpoint;
            if (string.IsNullOrEmpty(siAuthorizationEndpoint))
            {
                Result.ErrorMessage = "SI Authorization Endpoint is null or empty";
                return false;
            }

            return true;
        }

        private bool TryGetAuthReqId(SiAuthorizeResponse siAuthorizeResponse,
            out string authReqId)
        {
            authReqId =
                siAuthorizeResponse.Model?.AuthReqId;
            if (string.IsNullOrEmpty(authReqId))
            {
                Result.ErrorMessage = "AuthReqId is null or empty";
                return false;
            }

            return true;
        }
    }
}