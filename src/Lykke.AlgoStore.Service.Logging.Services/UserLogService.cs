using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Services.Strings;

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
            ValidateUserLogData(userLog);

            await _userLogRepository.WriteAsync(userLog);
        }

        public async Task WriteAsync(string instanceId, string message)
        {
            ValidateInstanceId(instanceId);
            ValidateMessage(message);

            await _userLogRepository.WriteAsync(instanceId, message);
        }

        public async Task WriteAsync(string instanceId, Exception ex)
        {
            ValidateInstanceId(instanceId);
            ValidateException(ex);

            await _userLogRepository.WriteAsync(instanceId, ex);
        }

        private static void ValidateInstanceId(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ValidationException(Phrases.InstanceIdCannotBeEmpty);
        }

        private static void ValidateMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ValidationException(Phrases.MessageCannotBeEmpty);
        }

        private static void ValidateException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));
        }

        private static void ValidateUserLogData([NotNull] IUserLog userLog)
        {
            if (userLog == null)
                throw new ArgumentNullException(nameof(userLog));

            ValidateInstanceId(userLog.InstanceId);
            ValidateMessage(userLog.Message);
        }
    }
}
