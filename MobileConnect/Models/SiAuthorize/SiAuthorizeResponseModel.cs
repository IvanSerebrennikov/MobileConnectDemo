using Newtonsoft.Json;

namespace MobileConnect.Models.SiAuthorize
{
    public class SiAuthorizeResponseModel
    {
        [JsonProperty("auth_req_id")]
        public string AuthReqId { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}