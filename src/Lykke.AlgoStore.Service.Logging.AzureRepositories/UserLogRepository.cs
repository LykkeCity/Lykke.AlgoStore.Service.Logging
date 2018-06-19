using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.DTOs;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Service.Logging.AzureRepositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly INoSQLTableStorage<UserLogEntity> _table;

        private readonly object _sync = new object();
        private long _lastDifference = -1;
        private int _duplicateCounter = 99999;

        public static readonly string TableName = "CSharpAlgoTemplateUserLog";

        public static string GeneratePartitionKey(string key) => key;

        public static string GenerateRowKey(long difference, int duplicateCounter) =>
            String.Format("{0:D19}{1:D5}_{2}", difference, duplicateCounter, Guid.NewGuid());

        public UserLogRepository(INoSQLTableStorage<UserLogEntity> table)
        {
            _table = table;
        }

        public async Task WriteAsync(IUserLog userLog)
        {
            var entity = Mapper.Map<UserLogEntity>(userLog);

            entity.PartitionKey = GeneratePartitionKey(userLog.InstanceId);
            entity.RowKey = GenerateRowKey();

            await _table.InsertAsync(entity);
        }

        public async Task WriteAsync(string instanceId, string message)
        {
            var entity = new UserLogEntity
            {
                Date = DateTime.UtcNow,
                Message = message,

                PartitionKey = GeneratePartitionKey(instanceId),
                RowKey = GenerateRowKey()
            };

            await _table.InsertAsync(entity);
        }

        public async Task WriteAsync(string instanceId, Exception ex)
        {
            await WriteAsync(instanceId, ex.ToString());
        }

        public async Task WriteAsync(IEnumerable<IUserLog> userLogs)
        {
            var batch = new TableBatchOperation();

            foreach (var userLog in userLogs)
            {
                var entity = Mapper.Map<UserLogEntity>(userLog);
                entity.PartitionKey = GeneratePartitionKey(userLog.InstanceId);
                entity.RowKey = GenerateRowKey();

                batch.Insert(entity);
            }

            await _table.DoBatchAsync(batch);
        }

        public async Task<IEnumerable<IUserLog>> GetAsync(int limit, string instanceId)
        {
            var query = new TableQuery<UserLogEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, GeneratePartitionKey(instanceId)))
                .Take(limit);

            var result = new List<UserLogDto>();

            await _table.ExecuteAsync(query, items => result.AddRange(Mapper.Map<IEnumerable<UserLogDto>>(items)), () => false);

            return result;
        }

        private string GenerateRowKey()
        {
            lock (_sync)
            {
                var difference = DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks;
                if (difference != _lastDifference)
                {
                    _lastDifference = difference;
                    _duplicateCounter = 99999;
                }
                else
                    _duplicateCounter -= 1;

                return GenerateRowKey(difference, _duplicateCounter);
            }
        }
    }
}
