using System;
using System.Linq;
using System.Threading.Tasks;
using MobileConnect.Interfaces;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;

namespace MobileConnect.Processors.SiAuthorize
{
    public class MobileConnectSiAuthorizeProcessor : IMobileConnectProcessor<MobileConnectSiAuthorizeResult, MobileConnectSiAuthorizeSettings>
    {
        private readonly MobileConnectSiAuthorizeResult _result = new MobileConnectSiAuthorizeResult();

        private MobileConnectClient _client;

        private MobileConnectSiAuthorizeSettings _settings;

        public string CorrelationId { get; }

        private string OpenIdConfigurationUrl { get; set; }

        private string ClientId { get; set; }

        private string Audience { get; set; }

        private string SiAuthorizationEndpoint { get; set; }

        public MobileConnectSiAuthorizeProcessor()
        {
            CorrelationId = Guid.NewGuid().ToString();
        }

        public bool SetClient(MobileConnectClient client)
        {
            if (_client != null) 
                return false;

            _client = client;
            return true;
        }

        public bool SetSettings(MobileConnectSiAuthorizeSettings settings)
        {
            if (_settings != null) 
                return false;

            _settings = settings;
            return true;
        }

        public async Task<MobileConnectSiAuthorizeResult> Process()
        {
            try
            {
                await ProcessDiscovery();
                await ProcessOpenIdConfiguration();
                await ProcessSiAuthorize();
            }
            catch (Exception e)
            {
                _result.ErrorMessage = e.Message;
            }

            return _result;
        }

        private async Task ProcessDiscovery()
        {
            if (!string.IsNullOrEmpty(_result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(CorrelationId))
                return;

            var discoveryRequest = new DiscoveryRequestModel
            {
                PhoneNumber = _settings.PhoneNumber,
                RedirectUrl = _settings.RedirectUrl,
                DiscoveryUrl = _settings.DiscoveryUrl,
                DiscoveryClientId = _settings.DiscoveryClientId,
                DiscoveryPassword = _settings.DiscoveryPassword,
                CorrelationId = CorrelationId
            };

            var discoveryResponse = await _client.SendDiscoveryRequest(discoveryRequest);
            if (discoveryResponse == null)
            {
                _result.ErrorMessage = "Discovery Response is null";
                return;
            }

            _result.DiscoveryResponse = discoveryResponse;

            if (!TryToSetOpenIdConfigurationUrl(discoveryResponse))
                return;

            if (!TryToSetClientId(discoveryResponse))
                return;
        }

        private async Task ProcessOpenIdConfiguration()
        {
            if (!string.IsNullOrEmpty(_result.ErrorMessage))
                return;

            if (string.IsNullOrEmpty(CorrelationId) ||
                string.IsNullOrEmpty(OpenIdConfigurationUrl))
                return;

            var openIdConfigurationRequestModel = new OpenIdConfigurationRequestModel
            {
                OpenIdConfigurationUrl = OpenIdConfigurationUrl
            };

            var openIdConfigurationResponse =
                await _client.SendOpenIdConfigurationRequest(openIdConfigurationRequestModel);
            if (openIdConfigurationResponse == null)
            {
                _result.ErrorMessage = "OpenId Configuration Response is null";
                return;
            }

            _result.OpenIdConfigurationResponse = openIdConfigurationResponse;

            if (!TryToSetAudience(openIdConfigurationResponse))
                return;

            if (!TryToSetSiAuthorizationEndpoint(openIdConfigurationResponse))
                return;
        }

        private async Task ProcessSiAuthorize()
        {
            if (!string.IsNullOrEmpty(_result.ErrorMessage))
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
                ResponseType = responseType,
                ClientId = ClientId,
                Scope = scope,
                RequestObjectClaims = new SiAuthorizeRequestObjectClaims
                {
                    ResponseType = responseType,
                    ClientId = ClientId,
                    Scope = scope,
                    Nonce = nonce,
                    LoginHint = $"MSISDN:{_settings.PhoneNumber}",
                    ArcValues = "3 2",
                    CorrelationId = CorrelationId,
                    Iss = ClientId,
                    Aud = Audience,
                    ClientNotificationToken = clientNotificationToken,
                    NotificationUri = _settings.NotificationUri,
                    Version = version
                }
            };

            var siAuthorizeResponse =
                await _client.SendSiAuthorizeRequest(siAuthorizeRequestModel, SiAuthorizationEndpoint, _settings.PrivateRsaKeyPath);
            if (siAuthorizeResponse == null)
            {
                _result.ErrorMessage = "SI Authorize Response is null";
                return;
            }

            _result.SiAuthorizeResponse = siAuthorizeResponse;
        }

        private bool TryToSetOpenIdConfigurationUrl(DiscoveryResponse discoveryResponse)
        {
            var openIdConfigurationRel = "openid-configuration";
            var openIdConfigurationUrl =
                discoveryResponse.Model?.Response?.Apis?.OperatorId?.Links.FirstOrDefault(x =>
                    x.Rel == openIdConfigurationRel)?.Href;
            if (string.IsNullOrEmpty(openIdConfigurationUrl))
            {
                _result.ErrorMessage = "OpenId Configuration Url is null or empty";
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
                _result.ErrorMessage = "ClientId is null or empty";
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
                _result.ErrorMessage = "Audience is null or empty";
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
                _result.ErrorMessage = "SI Authorization Endpoint is null or empty";
                return false;
            }

            SiAuthorizationEndpoint = siAuthorizationEndpoint;

            return true;
        }
    }
}