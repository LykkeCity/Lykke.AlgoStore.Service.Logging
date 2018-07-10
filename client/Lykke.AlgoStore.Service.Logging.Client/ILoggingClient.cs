
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.Service.Logging.Client
{
    public interface ILoggingClient
    {
        Task WriteAsync(UserLogRequest userLog, string instanceAuthtoken);
        Task WriteAsync(string instanceId, string message, string instanceAuthtoken);
        Task WriteAsync(IList<UserLogRequest> userLogs, string instanceAuthtoken);
        Task<IList<UserLogResponse>> GetTailLog(int tail, string instanceId, string instanceAuthtoken);
    }
}
