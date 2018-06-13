using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.AlgoStore.Service.Logging.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public LoggingSettings AlgoStoreLoggingService { get; set; }        
    }
}
