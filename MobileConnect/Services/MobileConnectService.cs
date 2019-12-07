using System.Threading.Tasks;
using MobileConnect.Interfaces;
using MobileConnect.Processors.SiAuthorize;

namespace MobileConnect.Services
{
    public class MobileConnectService : IMobileConnectService
    {
        private readonly MobileConnectClient _client;

        private readonly IMobileConnectProcessorsFactory _processorsFactory;

        public MobileConnectService()
        {
            // TODO: Get from ctor DI
            _client = new MobileConnectClient();
            _processorsFactory = new MobileConnectDefaultProcessorsFactory();
        }

        public async Task<MobileConnectSiAuthorizeResult> SiAuthorize(MobileConnectSiAuthorizeSettings settings)
        {
            var processor =
                _processorsFactory
                    .CreateProcessor<MobileConnectSiAuthorizeProcessor, MobileConnectSiAuthorizeResult,
                        MobileConnectSiAuthorizeSettings>(_client, settings);

            var result = await processor.Process();

            return result;
        }
    }
}