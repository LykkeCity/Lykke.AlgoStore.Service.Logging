using System;
using Autofac;
using Common.Log;

namespace Lykke.AlgoStore.Service.Logging.Client
{
    public static class AutofacExtension
    {
        public static void RegisterLoggingClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<LoggingClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ILoggingClient>()
                .SingleInstance();
        }

        public static void RegisterLoggingClient(this ContainerBuilder builder, LoggingServiceClientSettings settings, ILog log)
        {
            builder.RegisterLoggingClient(settings?.ServiceUrl, log);
        }
    }
}
