using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.Logging.Filters;
using Lykke.AlgoStore.Service.Logging.Modules;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.Common;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace Lykke.AlgoStore.Service.Logging
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        private ILog _log;
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = ApiName,
            ApiVersion = ApiVersion
        };
        private const string ApiVersion = "v1";
        private const string ApiName = "Algo Store Logging API";
        private string _monitoringServiceUrl;
        private IHealthNotifier _healthNotifier;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddSwaggerGen(options => { options.DefaultLykkeConfiguration(ApiVersion, ApiName); });

                var settingsManager = Configuration.LoadSettings<AppSettings>(x =>
                {
                    x.SetConnString(y => y.SlackNotifications.AzureQueue.ConnectionString);
                    x.SetQueueName(y => y.SlackNotifications.AzureQueue.QueueName);
                    x.SenderName = $"{AppEnvironment.Name} {AppEnvironment.Version}";
                });

                var appSettings = settingsManager.CurrentValue;

                if (appSettings.MonitoringServiceClient != null)
                    _monitoringServiceUrl = appSettings.MonitoringServiceClient.MonitoringServiceUrl;

                services.AddLykkeLogging(
                    settingsManager.ConnectionString(s => s.AlgoStoreLoggingService.Db.LogsConnectionString),
                    "AlgoStoreLoggingLog",
                    appSettings.SlackNotifications.AzureQueue.ConnectionString,
                    appSettings.SlackNotifications.AzureQueue.QueueName);

                services.AddInstanceAuthentication(appSettings.AlgoStoreLoggingService.LoggingServiceAuth);

                services.ConfigureSwaggerGen(options => options.OperationFilter<ApiKeyHeaderOperationFilter>());

                var builder = new ContainerBuilder();

                builder.RegisterModule(new ServiceModule(settingsManager));

                builder.Populate(services);

                ApplicationContainer = builder.Build();

                var logFactory = ApplicationContainer.Resolve<ILogFactory>();
                _log = logFactory.CreateLog(this);

                _healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                if (_log == null)
                    Console.WriteLine(ex);
                else
                    _log.Critical(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            try
            {
                app.UseAuthentication();

                app.UseLykkeConfiguration(options =>
                {
                    options.SwaggerOptions = _swaggerOptions;

                    options.DefaultErrorHandler = ex =>
                    {
                        string errorMessage;

                        switch (ex)
                        {
                            case InvalidOperationException ioe:
                                errorMessage = $"Invalid operation: {ioe.Message}";
                                break;
                            case ValidationException ve:
                                errorMessage = $"Validation error: {ve.Message}";
                                break;
                            default:
                                errorMessage = "Technical problem";
                                break;
                        }

                        return ErrorResponse.Create(errorMessage);
                    };
                });

                ConfigureAutoMapper();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(CleanUp);
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }

        private void ConfigureAutoMapper()
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfiles(typeof(AutoMapperProfile));
                    cfg.AddProfiles(typeof(AzureRepositories.AutoMapperProfile));
                    cfg.AddProfiles(typeof(AutoMapperModelProfile));
                });

                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception e)
            {
                _log.Error(nameof(ConfigureAutoMapper), e, "", nameof(Startup));

                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                _healthNotifier.Notify("Started", Program.EnvInfo);
#if !DEBUG
                await Configuration.RegisterInMonitoringServiceAsync(_monitoringServiceUrl, _healthNotifier);
#endif
            }
            catch (Exception ex)
            {
                _log.Critical(ex);
                throw;
            }
        }

        private void CleanUp()
        {
            try
            {
                // NOTE: Job can't receive and process IsAlive requests here, so you can destroy all resources
                _healthNotifier?.Notify("Terminating", Program.EnvInfo);

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }
    }
}
