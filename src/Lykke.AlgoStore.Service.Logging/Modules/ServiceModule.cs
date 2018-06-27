using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Services;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.SettingsReader;
using IUserLogRepository = Lykke.AlgoStore.Service.Logging.Core.Repositories.IUserLogRepository;
using UserLogRepository = Lykke.AlgoStore.Service.Logging.AzureRepositories.UserLogRepository;

namespace Lykke.AlgoStore.Service.Logging.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly ILog _log;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            var reloadingDbManager = _appSettings.ConnectionString(x => x.AlgoStoreLoggingService.Db.DataStorageConnectionString);

            builder.RegisterInstance(AzureTableStorage<UserLogEntity>.Create(reloadingDbManager, UserLogRepository.TableName, _log));

            builder.RegisterType<UserLogRepository>().As<IUserLogRepository>();

            builder.RegisterInstance<IAlgoClientInstanceRepository>(
                    AzureRepoFactories.CreateAlgoClientInstanceRepository(reloadingDbManager, _log))
                .SingleInstance();

            builder.RegisterType<UserLogService>()
                .As<IUserLogService>()
                .SingleInstance();
        }
    }
}
