using System.Collections.Generic;
using MobileConnectDemo.Helpers;

namespace MobileConnectDemo.Services.MobileConnect.Models
{
    public class SiAuthorizeRequestModel
    {
        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public SiAuthorizeRequestObjectClaims RequestObjectClaims { get; set; }
    }

    public class SiAuthorizeRequestObjectClaims
    {
        public string ResponseType { get; set; }

        public string ClientId { get; set; }

        public string Scope { get; set; }

        public string Nonce { get; set; }

        public string LoginHint { get; set; }

        public string ArcValues { get; set; }

        public string CorrelationId { get; set; }

        public string Iss { get; set; }

        public string Aud { get; set; }

        public string ClientNotificationToken { get; set; }

        public string NotificationUri { get; set; }

        public string Version { get; set; }

        public string ToJwtToken(string privateRsaKey)
        {
            var payload = new Dictionary<string, object>
            {
                {"response_type", ResponseType},
                {"client_id", ClientId},
                {"scope", Scope},
                {"nonce", Nonce},
                {"login_hint", LoginHint},
                {"acr_values", ArcValues},
                {"correlation_id", CorrelationId},
                {"iss", Iss},
                {"aud", Aud},
                {"client_notification_token", ClientNotificationToken},
                {"notification_uri", NotificationUri},
                {"version", Version}
            };

            return payload.ToJwtTokenWithRs256(privateRsaKey);
        }
    }
}