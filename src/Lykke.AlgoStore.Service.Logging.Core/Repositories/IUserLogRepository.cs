using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Logging.Core.Domain;

namespace Lykke.AlgoStore.Service.Logging.Core.Repositories
{
    public interface IUserLogRepository
    {
        Task WriteAsync(IUserLog userLog);
    }
}
