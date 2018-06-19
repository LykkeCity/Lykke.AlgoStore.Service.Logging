using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.Logging
{
    public class Startup
    {
        private ILog _log;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Algo Store Logging API";
                options.Logs = ("AlgoStoreLoggingLog", ctx => ctx.AlgoStoreLoggingService.Db.LogsConnectionString);
            });
        }

        public void Configure(IApplicationBuilder app, ILog log)
        {
            _log = log;

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
