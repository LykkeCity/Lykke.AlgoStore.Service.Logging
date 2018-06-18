using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Logging.Core.Domain;

namespace Lykke.AlgoStore.Service.Logging.Core.Repositories
{
    public interface IUserLogRepository
    {
        Task WriteAsync(IUserLog userLog);
        Task WriteAsync(string instanceId, string message);
        Task WriteAsync(string instanceId, Exception ex);
        Task WriteAsync(IEnumerable<IUserLog> userLogs);
    }
}
