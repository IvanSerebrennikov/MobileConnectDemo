using System.Threading.Tasks;

namespace MobileConnect.Interfaces
{
    public interface IMobileConnectProcessor
    {
        void SetClient(MobileConnectClient client);

        void SetSettings(IMobileConnectProcessorSettings settings);

        Task<IMobileConnectProcessResult> ProcessAndGetResult();
    }
}