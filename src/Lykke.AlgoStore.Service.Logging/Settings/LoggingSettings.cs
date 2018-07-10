using JetBrains.Annotations;
using Lykke.AlgoStore.Security.InstanceAuth;

namespace Lykke.AlgoStore.Service.Logging.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LoggingSettings
    {
        public DbSettings Db { get; set; }
        public InstanceAuthSettings LoggingServiceAuth { get; set; }
    }
}
