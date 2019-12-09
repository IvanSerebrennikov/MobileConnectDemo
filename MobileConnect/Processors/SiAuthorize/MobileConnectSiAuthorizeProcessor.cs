using System;
using System.Linq;
using System.Threading.Tasks;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;
using MobileConnect.Processors.Base;

namespace MobileConnect.Processors.SiAuthorize
{
    public class MobileConnectSiAuthorizeProcessor : MobileConnectProcessor<MobileConnectSiAuthorizeResult, MobileConnectSiAuthorizeSettings>
    {
        public string CorrelationId { get; }

        private string OpenIdConfigurationUrl { get; set; }

        private string ClientId { get; set; }

        private string Audience { get; set; }

        private string SiAuthorizationEndpoint { get; set; }

        public MobileConnectSiAuthorizeProcessor()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        protected override async Task Process()
        {
            await ProcessDiscovery();
            await ProcessOpenIdConfiguration();
            await ProcessSiAuthorize();
        }

        private async Task ProcessDiscovery()
        {
            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(CorrelationId))
                return;

            var discoveryRequest = new DiscoveryRequestModel
            {
                PhoneNumber = Settings.PhoneNumber,
                RedirectUrl = Settings.RedirectUrl,
                DiscoveryUrl = Settings.DiscoveryUrl,
                DiscoveryClientId = Settings.DiscoveryClientId,
                DiscoveryPassword = Settings.DiscoveryPassword,
                CorrelationId = CorrelationId
            };

            var discoveryResponse = await Client.SendDiscoveryRequest(discoveryRequest);
            if (discoveryResponse == null)
            {
                Result.ErrorMessage = "Discovery Response is null";
                return;
            }

            Result.DiscoveryResponse = discoveryResponse;

            if (!TryToSetOpenIdConfigurationUrl(discoveryResponse))
                return;

            if (!TryToSetClientId(discoveryResponse))
                return;
        }

        private async Task ProcessOpenIdConfiguration()
        {
            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(CorrelationId) ||
                string.IsNullOrEmpty(OpenIdConfigurationUrl))
                return;

            var openIdConfigurationRequestModel = new OpenIdConfigurationRequestModel
            {
                OpenIdConfigurationUrl = OpenIdConfigurationUrl
            };

            var openIdConfigurationResponse =
                await Client.SendOpenIdConfigurationRequest(openIdConfigurationRequestModel);
            if (openIdConfigurationResponse == null)
            {
                Result.ErrorMessage = "OpenId Configuration Response is null";
                return;
            }

            Result.OpenIdConfigurationResponse = openIdConfigurationResponse;

            if (!TryToSetAudience(openIdConfigurationResponse))
                return;

            if (!TryToSetSiAuthorizationEndpoint(openIdConfigurationResponse))
                return;
        }

        private async Task ProcessSiAuthorize()
        {
            if (!string.IsNullOrEmpty(Result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(CorrelationId) ||
                string.IsNullOrEmpty(ClientId) ||
                string.IsNullOrEmpty(Audience) ||
                string.IsNullOrEmpty(SiAuthorizationEndpoint))
                return;

            var nonce = Guid.NewGuid().ToString();
            var clientNotificationToken = Guid.NewGuid().ToString();

            var responseType = "mc_si_async_code";
            var scope = "openid mc_identity_phonenumber";

            var version = "mc_si_v2.0";

            var siAuthorizeRequestModel = new SiAuthorizeRequestModel
            {
                SiAuthorizationUrl = SiAuthorizationEndpoint,
                PrivateRsaKeyPath = Settings.PrivateRsaKeyPath,
                ResponseType = responseType,
                ClientId = ClientId,
                Scope = scope,
                RequestObjectClaims = new SiAuthorizeRequestObjectClaims
                {
                    ResponseType = responseType,
                    ClientId = ClientId,
                    Scope = scope,
                    Nonce = nonce,
                    LoginHint = $"MSISDN:{Settings.PhoneNumber}",
                    ArcValues = "3 2",
                    CorrelationId = CorrelationId,
                    Iss = ClientId,
                    Aud = Audience,
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
        }

        private bool TryToSetOpenIdConfigurationUrl(DiscoveryResponse discoveryResponse)
        {
            var openIdConfigurationRel = "openid-configuration";
            var openIdConfigurationUrl =
                discoveryResponse.Model?.Response?.Apis?.OperatorId?.Links.FirstOrDefault(x =>
                    x.Rel == openIdConfigurationRel)?.Href;
            if (string.IsNullOrEmpty(openIdConfigurationUrl))
            {
                Result.ErrorMessage = "OpenId Configuration Url is null or empty";
                return false;
            }

            OpenIdConfigurationUrl = openIdConfigurationUrl;
            return true;
        }

        private bool TryToSetClientId(DiscoveryResponse discoveryResponse)
        {
            var clientId =
                discoveryResponse.Model?.Response?.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                Result.ErrorMessage = "ClientId is null or empty";
                return false;
            }

            ClientId = clientId;

            return true;
        }

        private bool TryToSetAudience(OpenIdConfigurationResponse openIdConfigurationResponse)
        {
            var audience =
                openIdConfigurationResponse.Model?.Issuer;
            if (string.IsNullOrEmpty(audience))
            {
                Result.ErrorMessage = "Audience is null or empty";
                return false;
            }

            Audience = audience;

            return true;
        }

        private bool TryToSetSiAuthorizationEndpoint(OpenIdConfigurationResponse openIdConfigurationResponse)
        {
            var siAuthorizationEndpoint =
                openIdConfigurationResponse.Model?.SiAuthorizationEndpoint;
            if (string.IsNullOrEmpty(siAuthorizationEndpoint))
            {
                Result.ErrorMessage = "SI Authorization Endpoint is null or empty";
                return false;
            }

            SiAuthorizationEndpoint = siAuthorizationEndpoint;

            return true;
        }
    }
}