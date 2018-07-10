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

        public async Task WriteAsync(UserLogRequest userLog, string instanceAuthToken)
        {
            await _service.WriteLogWithHttpMessagesAsync(userLog, SetAutorizationToken(instanceAuthToken));
        }

        public async Task WriteAsync(string instanceId, string message, string instanceAuthToken)
        {
            await _service.WriteMessageWithHttpMessagesAsync(instanceId, message, SetAutorizationToken(instanceAuthToken));
        }

        public async Task WriteAsync(IList<UserLogRequest> userLogs, string instanceAuthToken)
        {
            await _service.WriteLogsWithHttpMessagesAsync(userLogs, SetAutorizationToken(instanceAuthToken));
        }

        public async Task<IList<UserLogResponse>> GetTailLog(int tail, string instanceId, string instanceAuthToken)
        {
            return (await _service
                .GetTailLogWithHttpMessagesAsync(tail, instanceId, SetAutorizationToken(instanceAuthToken))
                .ConfigureAwait(false)).Body;
        }

        private Dictionary<string, List<string>> SetAutorizationToken(string authToken)
        {
            var result = new Dictionary<string, List<string>>
                {{"Authorization", new List<string>() {"Bearer " + authToken}}};

            return result;
        }
    }
}
