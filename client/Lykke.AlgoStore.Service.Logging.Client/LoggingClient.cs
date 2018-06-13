using System;
using System.Net.Http;
using Common.Log;
using Lykke.Service.Logging.Client.AutorestClient;

namespace Lykke.AlgoStore.Service.Logging.Client
{
    public class LoggingClient : ILoggingClient, IDisposable
    {
        private readonly ILog _log;
        private IAlgoStoreLoggingAPI _service;

        public LoggingClient(string serviceUrl, ILog log)
        {
            _log = log;
            _service = new AlgoStoreLoggingAPI(new Uri(serviceUrl), new HttpClient());
        }

        public void Dispose()
        {
            if (_service == null)
                return;
            _service.Dispose();
            _service = null;
        }
    }
}
