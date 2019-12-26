using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MobileConnect.Interfaces;

namespace MobileConnect.Processors.Base
{
    public abstract class MobileConnectProcessor<TResult, TSettings> : IMobileConnectProcessor
        where TResult : class, IMobileConnectProcessResult, new()
        where TSettings : IMobileConnectProcessorSettings
    {
        protected readonly TResult Result = new TResult();

        private MobileConnectClient _client;

        private TSettings _settings;

        protected MobileConnectClient Client
        {
            get => _client;
            set
            {
                if (_client == null)
                {
                    _client = value;
                }
            }
        }

        protected TSettings Settings
        {
            get => _settings;
            set
            {
                if (_settings == null)
                {
                    _settings = value;
                }
            }
        }

        public void SetClient(MobileConnectClient client)
        {
            Client = client;
        }

        public void SetSettings(IMobileConnectProcessorSettings settings)
        {
            if (settings is TSettings actualSettings)
                Settings = actualSettings;
            else
                throw new InvalidEnumArgumentException($"Wrong settings type {settings.GetType()}");
        }

        public async Task<IMobileConnectProcessResult> ProcessAndGetResult()
        {
            try
            {
                await Process();
            }
            catch (Exception e)
            {
                Result.ErrorMessage = $"Exception! Message: {e.Message}; Inner: {e.InnerException?.Message}";
            }

            return Result;
        }

        protected abstract Task Process();
    }
}