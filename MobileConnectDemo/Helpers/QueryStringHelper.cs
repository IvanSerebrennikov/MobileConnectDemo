using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MobileConnectDemo.Helpers
{
    public static class QueryStringHelper
    {
        public static string ToQueryString(this Dictionary<string, object> parameters)
        {
            if (!parameters.Any())
                return "";

            var builder = new StringBuilder("?");

            var index = 0;
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null)
                    continue;

                if (index != 0)
                    builder.Append("&");

                builder.AppendFormat(
                    "{0}={1}",
                    WebUtility.UrlEncode(kvp.Key),
                    WebUtility.UrlEncode(kvp.Value.ToString()));

                index++;
            }

            return builder.ToString();
        }
    }
}