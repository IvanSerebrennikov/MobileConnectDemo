using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileConnect.Models.Base
{
    public class BaseMobileConnectResponse<T>
    {
        public T Model { get; set; }

        public string JsonString { get; set; }

        public bool IsSucceeded { get; set; }
    }
}
