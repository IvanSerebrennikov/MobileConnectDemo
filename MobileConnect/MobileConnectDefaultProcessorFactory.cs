using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileConnect.Interfaces;

namespace MobileConnect
{
    public class MobileConnectDefaultProcessorFactory : IMobileConnectProcessorFactory
    {
        public IMobileConnectProcessor<TResult, TSettings> CreateProcessor<TProcessor, TResult, TSettings>(MobileConnectClient client, TSettings settings)
            where TProcessor : IMobileConnectProcessor<TResult, TSettings>, new()
            where TResult : IMobileConnectProcessResult
            where TSettings : IMobileConnectProcessorSettings
        {
            var processor = new TProcessor();

            processor.SetClient(client);

            processor.SetSettings(settings);

            return processor;
        }
    }
}
