using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileConnectDemo.Services.MobileConnect.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Services.MobileConnect
{
    public class MobileConnectService : IMobileConnectService
    {
        public async Task<MobileConnectAuthorizeResult> SiAuthorize(MobileConnectAuthorizeSettings settings)
        {
            var result = new MobileConnectAuthorizeResult();

            try
            {
                var correlationId = Guid.NewGuid().ToString();

                var discoveryRequest = new DiscoveryRequestModel
                {
                    PhoneNumber = settings.PhoneNumber,
                    RedirectUrl = settings.RedirectUrl,
                    DiscoveryUrl = settings.DiscoveryUrl,
                    DiscoveryClientId = settings.DiscoveryClientId,
                    DiscoveryPassword = settings.DiscoveryPassword,
                    CorrelationId = correlationId
                };

                var discoveryResponse = await SendDiscoveryRequest(discoveryRequest);

                if (discoveryResponse == null)
                {
                    result.ErrorMessage = "Discovery Response is null";

                    return result;
                }

                result.DiscoveryResponse = discoveryResponse;

                var openIdConfigurationRel = "openid-configuration";
                var openIdConfigurationUrl =
                    discoveryResponse.Model?.Response?.Apis?.OperatorId?.Links.FirstOrDefault(x =>
                        x.Rel == openIdConfigurationRel)?.Href;

                if (string.IsNullOrEmpty(openIdConfigurationUrl))
                {
                    result.ErrorMessage = "OpenId Configuration Url is null or empty";

                    return result;
                }

                var openIdConfigurationRequestModel = new OpenIdConfigurationRequestModel
                {
                    OpenIdConfigurationUrl = openIdConfigurationUrl
                };

                var openIdConfigurationResponse =
                    await SendOpenIdConfigurationRequest(openIdConfigurationRequestModel);

                if (openIdConfigurationResponse == null)
                {
                    result.ErrorMessage = "OpenId Configuration Response is null";

                    return result;
                }

                result.OpenIdConfigurationResponse = openIdConfigurationResponse;
            }
            catch (Exception e)
            {
                result.ErrorMessage = e.Message;
            }

            return result;
        }

        private async Task<DiscoveryResponse> SendDiscoveryRequest(DiscoveryRequestModel requestModel)
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

        private async Task<OpenIdConfigurationResponse> SendOpenIdConfigurationRequest(
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
    }
}