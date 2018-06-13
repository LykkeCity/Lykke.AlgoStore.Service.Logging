using AzureStorage;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;

namespace Lykke.AlgoStore.Service.Logging.AzureRepositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly INoSQLTableStorage<UserLogEntity> _table;

        public UserLogRepository(INoSQLTableStorage<UserLogEntity> table)
        {
            _table = table;
        }
    }
}
