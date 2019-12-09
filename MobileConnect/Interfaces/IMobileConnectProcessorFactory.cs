namespace MobileConnect.Interfaces
{
    public interface IMobileConnectProcessorFactory
    {
        IMobileConnectProcessor CreateProcessor<TProcessor>(
            MobileConnectClient client, IMobileConnectProcessorSettings settings)
            where TProcessor : IMobileConnectProcessor, new();
    }
}