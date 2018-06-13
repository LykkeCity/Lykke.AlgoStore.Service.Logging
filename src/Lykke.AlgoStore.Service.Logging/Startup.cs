using System;
using Lykke.AlgoStore.Service.Logging.Settings;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.Service.Logging
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Algo Store Logging API";
                options.Logs = ("AlgoStoreLoggingLog", ctx => ctx.AlgoStoreLoggingService.Db.LogsConnectionString);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();

        }
    }
}
