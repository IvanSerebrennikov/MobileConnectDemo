using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.DataAccess.Entities
{
    public class MobileConnectAuthorizeRequest
    {
        public MobileConnectAuthorizeRequest()
        {
            Id = new Random().Next(100000, 999999);
        }

        public int Id { get; }

        public string PhoneNumber { get; set; }

        public string ClientNotificationToken { get; set; }

        public string AuthReqId { get; set; }

        public string CorrelationId { get; set; }

        public string Nonce { get; set; }

        public bool IsResponseSucceeded { get; set; }

        public string ResponseErrorMessage { get; set; }

        public bool? IsNotificationReceived { get; set; }

        public bool? IsAuthorized { get; set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as MobileConnectAuthorizeRequest;

            if (item == null)
                return false;

            return this.Id == item.Id;
        }
    }
}