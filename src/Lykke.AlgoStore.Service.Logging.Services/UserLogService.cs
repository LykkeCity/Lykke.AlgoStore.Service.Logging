using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Lykke.AlgoStore.Service.Logging.Core.Services;

namespace Lykke.AlgoStore.Service.Logging.Services
{
    public class UserLogService : IUserLogService
    {
        private readonly IUserLogRepository _userLogRepository;

        public UserLogService([NotNull] IUserLogRepository userLogRepository)
        {
            _userLogRepository = userLogRepository ?? throw new ArgumentNullException(nameof(userLogRepository));
        }

        public async Task WriteAsync(IUserLog userLog)
        {
            await _userLogRepository.WriteAsync(userLog);
        }

        public async Task WriteAsync(string instanceId, string message)
        {
            await _userLogRepository.WriteAsync(instanceId, message);
        }

        public async Task WriteAsync(string instanceId, Exception ex)
        {
            await _userLogRepository.WriteAsync(instanceId, ex);
        }
    }
}
