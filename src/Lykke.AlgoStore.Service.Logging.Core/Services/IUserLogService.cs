using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Logging.Core.Domain;

namespace Lykke.AlgoStore.Service.Logging.Core.Services
{
    public interface IUserLogService
    {
        Task WriteAsync(IUserLog userLog);
        Task WriteAsync(string instanceId, string message);
        Task WriteAsync(string instanceId, Exception ex);
    }
}
