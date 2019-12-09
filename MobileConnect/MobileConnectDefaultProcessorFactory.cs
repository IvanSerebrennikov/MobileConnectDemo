using MobileConnect.Interfaces;

namespace MobileConnect
{
    public class MobileConnectDefaultProcessorFactory : IMobileConnectProcessorFactory
    {
        public IMobileConnectProcessor CreateProcessor<TProcessor>(MobileConnectClient client,
            IMobileConnectProcessorSettings settings)
            where TProcessor : IMobileConnectProcessor, new()
        {
            var processor = new TProcessor();

            processor.SetClient(client);

            processor.SetSettings(settings);

            return processor;
        }
    }
}