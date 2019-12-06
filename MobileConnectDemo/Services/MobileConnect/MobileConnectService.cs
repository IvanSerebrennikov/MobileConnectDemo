using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MobileConnectDemo.Helpers;
using MobileConnectDemo.Services.MobileConnect.Interfaces;
using MobileConnectDemo.Services.MobileConnect.Models;
using Newtonsoft.Json;

namespace MobileConnectDemo.Services.MobileConnect
{
    public class MobileConnectService : IMobileConnectService
    {
        private readonly MobileConnectClient _client;

        public MobileConnectService()
        {
            _client = new MobileConnectClient();
        }

        public async Task<MobileConnectAuthorizeResult> SiAuthorize(MobileConnectAuthorizeSettings settings)
        {
            var processor = new MobileConnectAuthorizeProcessor(_client, settings);

            var result = await processor.Process();

            return result;
        }
    }
}