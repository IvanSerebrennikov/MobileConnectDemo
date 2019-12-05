using System;
using System.Collections.Generic;
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
        public async Task<DiscoveryResponse> SendDiscoveryRequest(DiscoveryRequestModel requestModel)
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
                    { "Redirect_URL", requestModel.RedirectUrl },
                    { "MSISDN", requestModel.PhoneNumber },
                    { "correlation_id", requestModel.CorrelationId }
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

        public async Task<string> SendOpenIdConfigurationRequest(string openIdConfigurationUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var responseString = await httpClient.GetStringAsync(openIdConfigurationUrl);

                return responseString;
            }
        }
    }
}