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
            using (var handler = new WebRequestHandler())
            {
                if (requestModel.AllowSelfHostedCertificates)
                {
                    handler.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;
                }
                
                using (var httpClient = new HttpClient(handler))
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

                    var isSucceeded = response.IsSuccessStatusCode;

                    var responseModel = isSucceeded
                        ? JsonConvert.DeserializeObject<DiscoveryResponseModel>(responseString)
                        : null;

                    return new DiscoveryResponse
                    {
                        Model = responseModel,
                        JsonString = responseString,
                        IsSucceeded = isSucceeded
                    };
                }
            }
        }

        public async Task<OpenIdConfigurationResponse> SendOpenIdConfigurationRequest(
            OpenIdConfigurationRequestModel requestModel)
        {
            using (var handler = new WebRequestHandler())
            {
                if (requestModel.AllowSelfHostedCertificates)
                {
                    handler.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;
                }
                
                using (var httpClient = new HttpClient(handler))
                {
                    var response = await httpClient.GetAsync(requestModel.OpenIdConfigurationUrl);

                    var responseString = await response.Content.ReadAsStringAsync();

                    var isSucceeded = response.IsSuccessStatusCode;

                    var responseModel = isSucceeded
                        ? JsonConvert.DeserializeObject<OpenIdConfigurationResponseModel>(responseString)
                        : null;

                    return new OpenIdConfigurationResponse
                    {
                        Model = responseModel,
                        JsonString = responseString,
                        IsSucceeded = isSucceeded
                    };
                }
            }
        }

        public async Task<SiAuthorizeResponse> SendSiAuthorizeRequest(
            SiAuthorizeRequestModel requestModel)
        {
            var privateRsaKey = File.ReadAllText(requestModel.PrivateRsaKeyPath);

            using (var handler = new WebRequestHandler())
            {
                if (requestModel.AllowSelfHostedCertificates)
                {
                    handler.ServerCertificateValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;
                }
                
                using (var httpClient = new HttpClient(handler))
                {
                    var values = new Dictionary<string, object>
                    {
                        {"response_type", requestModel.ResponseType},
                        {"client_id", requestModel.ClientId},
                        {"scope", requestModel.Scope},
                        {"request", requestModel.RequestObjectClaims.ToJwtTokenWithRs256(privateRsaKey)}
                    };

                    var response = await httpClient.GetAsync(
                        $"{requestModel.SiAuthorizationUrl}{values.ToQueryString()}");

                    var responseString = await response.Content.ReadAsStringAsync();

                    var isSucceeded = response.IsSuccessStatusCode;

                    var responseModel = isSucceeded
                        ? JsonConvert.DeserializeObject<SiAuthorizeResponseModel>(responseString)
                        : null;

                    return new SiAuthorizeResponse
                    {
                        Model = responseModel,
                        JsonString = responseString,
                        IsSucceeded = isSucceeded
                    };
                }
            }
        }
    }
}