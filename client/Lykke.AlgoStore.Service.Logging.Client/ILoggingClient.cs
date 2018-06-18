
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Logging.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.Service.Logging.Client
{
    public interface ILoggingClient
    {
        Task WriteAsync(UserLogRequest userLog);
        Task WriteAsync(string instanceId, string message);
        Task WriteAsync(IList<UserLogRequest> userLogs);
    }
}
