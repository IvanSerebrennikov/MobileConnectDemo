using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace MobileConnectDemo.Logger
{
    public class FakeLogger
    {
        public void Info(string message)
        {
            Debug.WriteLine($"Info: {message}");
        }

        public void Warn(string message)
        {
            Debug.WriteLine($"Warn: {message}");
        }
    }
}