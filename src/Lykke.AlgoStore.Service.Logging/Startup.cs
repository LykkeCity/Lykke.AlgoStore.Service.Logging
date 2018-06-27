using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Security.InstanceAuth;
using Lykke.AlgoStore.Service.Logging.Filters;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.Logging
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private ILog _log;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.LoadSettings<AppSettings>();

            services.AddInstanceAuthentication(appSettings.CurrentValue.AlgoStoreLoggingService.LoggingServiceCache);

            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Algo Store Logging API";
                options.Logs = ("AlgoStoreLoggingLog", ctx => ctx.AlgoStoreLoggingService.Db.LogsConnectionString);
            }, swagger => { swagger.OperationFilter<ApiKeyHeaderOperationFilter>(); });
        }

        public void Configure(IApplicationBuilder app, ILog log)
        {
            _log = log;

            app.UseAuthentication();

            app.UseLykkeConfiguration(ex =>
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
            });

            ConfigureAutoMapper();
        }

        private void ConfigureAutoMapper()
        {
            try
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfiles(typeof(AutoMapperProfile));
                    cfg.AddProfiles(typeof(AzureRepositories.AutoMapperProfile));
                });

                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception e)
            {
                _log?.WriteFatalErrorAsync(nameof(Startup), nameof(ConfigureAutoMapper), "", e).Wait();

                throw;
            }
        }
    }
}
