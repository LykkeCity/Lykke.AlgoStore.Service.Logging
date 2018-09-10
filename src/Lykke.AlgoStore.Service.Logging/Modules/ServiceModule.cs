using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Services;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Logs.Loggers.LykkeConsole;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;
using IUserLogRepository = Lykke.AlgoStore.Service.Logging.Core.Repositories.IUserLogRepository;
using UserLogRepository = Lykke.AlgoStore.Service.Logging.AzureRepositories.UserLogRepository;

namespace Lykke.AlgoStore.Service.Logging.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public ServiceModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them
            var logFactory = LogFactory.Create().AddConsole();
            var reloadingDbManager = _appSettings.ConnectionString(x => x.AlgoStoreLoggingService.Db.DataStorageConnectionString);

            builder.RegisterInstance(AzureTableStorage<UserLogEntity>.Create(reloadingDbManager, UserLogRepository.TableName, logFactory));

            builder.RegisterType<UserLogRepository>().As<IUserLogRepository>();

            builder.Register(x =>
                {
                    var log = x.Resolve<ILogFactory>();
                    var repository = CSharp.AlgoTemplate.Models.AzureRepoFactories
                        .CreateAlgoClientInstanceRepository(
                            _appSettings.Nested(r => r.AlgoStoreLoggingService.Db.DataStorageConnectionString),
                            log.CreateLog(this));

                    return repository;
                })
                .As<IAlgoClientInstanceRepository>()
                .SingleInstance();

            builder.RegisterType<UserLogService>()
                .As<IUserLogService>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();
        }
    }
}
