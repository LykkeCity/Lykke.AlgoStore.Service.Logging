using System;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;

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

            entity.PartitionKey = GeneratePartitionKey(entity.InstanceId);
            entity.RowKey = GenerateRowKey();

            await _table.InsertAsync(entity);
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
