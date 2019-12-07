using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileConnect.Helpers;
using MobileConnect.Models.Discovery;
using MobileConnect.Models.OpenIdConfiguration;
using MobileConnect.Models.SiAuthorize;
using Newtonsoft.Json;

namespace MobileConnect
{
    public class MobileConnectClient
    {
        public async Task<DiscoveryResponse> SendDiscoveryRequest(
            DiscoveryRequestModel requestModel)
        {
            using (var httpClient = new HttpClient())
            {
                var authenticationHeaderValue = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(
                            $"{requestModel.DiscoveryClientId}:{requestModel.DiscoveryPassword}")));

                httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                var values = new Dictionary<string, string>
                {
                    {"Redirect_URL", requestModel.RedirectUrl},
                    {"MSISDN", requestModel.PhoneNumber},
                    {"correlation_id", requestModel.CorrelationId}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await httpClient.PostAsync(requestModel.DiscoveryUrl, content);

                var responseString = await response.Content.ReadAsStringAsync();

                var responseModel = JsonConvert.DeserializeObject<DiscoveryResponseModel>(responseString);

                return new DiscoveryResponse
                {
                    Model = responseModel,
                    JsonString = responseString
                };
            }
        }

        public async Task<OpenIdConfigurationResponse> SendOpenIdConfigurationRequest(
            OpenIdConfigurationRequestModel requestModel)
        {
            using (var httpClient = new HttpClient())
            {
                var responseString = await httpClient.GetStringAsync(requestModel.OpenIdConfigurationUrl);

                var responseModel = JsonConvert.DeserializeObject<OpenIdConfigurationResponseModel>(responseString);

                return new OpenIdConfigurationResponse
                {
                    Model = responseModel,
                    JsonString = responseString
                };
            }
        }

        public async Task<SiAuthorizeResponse> SendSiAuthorizeRequest(
            SiAuthorizeRequestModel requestModel, string siAuthorizationEndpoint, string privateRsaKeyPath)
        {
            var privateRsaKey = File.ReadAllText(privateRsaKeyPath);

            using (var httpClient = new HttpClient())
            {
                var values = new Dictionary<string, object>
                {
                    {"response_type", requestModel.ResponseType},
                    {"client_id", requestModel.ClientId},
                    {"scope", requestModel.Scope},
                    {"request", requestModel.RequestObjectClaims.ToJwtTokenWithRs256(privateRsaKey)}
                };

                var response = await httpClient.GetAsync(
                    $"{siAuthorizationEndpoint}{values.ToQueryString()}");

                var responseString = await response.Content.ReadAsStringAsync();

                var responseModel = JsonConvert.DeserializeObject<SiAuthorizeResponseModel>(responseString);

                return new SiAuthorizeResponse
                {
                    Model = responseModel,
                    JsonString = responseString
                };
            }
        }
    }
}