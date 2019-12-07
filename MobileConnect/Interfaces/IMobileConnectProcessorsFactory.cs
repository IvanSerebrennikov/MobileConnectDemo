namespace MobileConnect.Interfaces
{
    public interface IMobileConnectProcessorsFactory
    {
        IMobileConnectProcessor<TResult, TSettings> CreateProcessor<TProcessor, TResult, TSettings>(
            MobileConnectClient client, TSettings settings)
            where TProcessor : IMobileConnectProcessor<TResult, TSettings>, new()
            where TResult : IMobileConnectProcessResult
            where TSettings : IMobileConnectProcessorSettings;
    }
}