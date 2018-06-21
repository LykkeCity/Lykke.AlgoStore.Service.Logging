using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.Logging.Client.AutorestClient;
using Lykke.Service.Logging.Client.AutorestClient.Models;

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

        public async Task WriteAsync(UserLogRequest userLog)
        {
            await _service.WriteLogAsync(userLog);
        }

        public async Task WriteAsync(string instanceId, string message)
        {
            await _service.WriteMessageAsync(instanceId, message);
        }

        public async Task WriteAsync(IList<UserLogRequest> userLogs)
        {
            await _service.WriteLogsAsync(userLogs);
        }

        public async Task<TailLogResponse> GetTailLog(int tail, string instanceId)
        {
            return await _service.GetTailLogAsync(tail, instanceId);
        }
    }
}
