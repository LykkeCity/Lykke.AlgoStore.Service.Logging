using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.AlgoStore.Service.Logging.Core;
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

        public async Task WriteAsync(IEnumerable<IUserLog> userLogs)
        {
            var logs = userLogs.ToList();

            ValidateUserLogs(logs);

            await _userLogRepository.WriteAsync(logs);
        }

        public async Task<string[]> GetTailLog(int limit, string instanceId)
        {
            ValidateInstanceId(instanceId);
            ValidateLimit(limit);

            var userLogs = await _userLogRepository.GetAsync(limit, instanceId);

            return userLogs.Select(l => $"[{l.Date.ToString(Constants.CustomDateTimeFormat)}] {l.Message}").ToArray();
        }

        private static void ValidateLimit(int limit)
        {
            if(limit <= 0)
                throw new ValidationException(Phrases.LogNumberOfReturnedRecordsLimitReached);
        }

        private static void ValidateUserLogs([NotNull] IEnumerable<IUserLog> userLogs)
        {
            if (userLogs == null)
                throw new ArgumentNullException(nameof(userLogs));

            var logs = userLogs.ToList();

            if(logs.Count > 100)
                throw new ValidationException(Phrases.InstanceIdMustBeSameForAllLogs);

            if(logs.Select(x => x.InstanceId).Distinct().Count() > 1)
                throw new ValidationException(Phrases.MaxNumberOfLogsPerBatchReached);

            if (logs.Select(x => x.Message).Any(x => x != null && string.IsNullOrEmpty(x)))
                throw new ValidationException(Phrases.AnyMessageCanNotBeEmpty);
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
