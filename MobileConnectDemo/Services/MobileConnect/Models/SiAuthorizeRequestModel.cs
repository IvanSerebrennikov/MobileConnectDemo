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
    }
}