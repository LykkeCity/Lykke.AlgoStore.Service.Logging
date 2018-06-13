using System;
using Common.Log;

namespace Lykke.AlgoStore.Service.Logging.Client
{
    public class LoggingClient : ILoggingClient, IDisposable
    {
        private readonly ILog _log;

        public LoggingClient(string serviceUrl, ILog log)
        {
            _log = log;
        }

        public void Dispose()
        {
            //if (_service == null)
            //    return;
            //_service.Dispose();
            //_service = null;
        }
    }
}
