using System.Threading.Tasks;
using MobileConnect.Interfaces.Services;
using MobileConnect.Processors.SiAuthorize;

namespace MobileConnect.Services
{
    public class MobileConnectService : IMobileConnectService
    {
        private readonly MobileConnectClient _client;

        public MobileConnectService()
        {
            _client = new MobileConnectClient();
        }

        public async Task<MobileConnectSiAuthorizeResult> SiAuthorize(MobileConnectSiAuthorizeSettings settings)
        {
            var processor = new MobileConnectSiAuthorizeProcessor(_client, settings);

            var result = await processor.Process();

            return result;
        }
    }
}