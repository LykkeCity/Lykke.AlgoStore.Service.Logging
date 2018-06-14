using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AlgoStore.Service.Logging.AzureRepositories;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Repositories;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Services;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.Logging.Modules
{    
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;
        private readonly ILog _log;
        //private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
            //_services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Do not register entire settings in container, pass necessary settings to services which requires them

            var reloadingDbManager = _appSettings.ConnectionString(x => x.AlgoStoreLoggingService.Db.DataStorageConnectionString);

            builder.RegisterInstance(AzureTableStorage<UserLogEntity>.Create(reloadingDbManager, UserLogRepository.TableName, _log));

            builder.RegisterType<UserLogRepository>().As<IUserLogRepository>();

            builder.RegisterType<UserLogService>()
                .As<IUserLogService>()
                .SingleInstance();

            //builder.Populate(_services);
        }
    }
}
