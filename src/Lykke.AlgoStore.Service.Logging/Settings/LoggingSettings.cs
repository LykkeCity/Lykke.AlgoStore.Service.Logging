using JetBrains.Annotations;

namespace Lykke.AlgoStore.Service.Logging.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class LoggingSettings
    {
        public DbSettings Db { get; set; }
    }
}
