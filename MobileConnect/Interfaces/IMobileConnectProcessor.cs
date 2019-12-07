using System.Threading.Tasks;

namespace MobileConnect.Interfaces
{
    public interface IMobileConnectProcessor<TResult, TSettings>
        where TResult : IMobileConnectProcessResult
        where TSettings : IMobileConnectProcessorSettings
    {
        bool SetClient(MobileConnectClient client);

        bool SetSettings(TSettings settings);

        Task<TResult> Process();
    }
}