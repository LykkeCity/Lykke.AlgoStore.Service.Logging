using System;

namespace Lykke.AlgoStore.Service.Logging.Core.Domain
{
    public interface IUserLog
    {
        string InstanceId { get; }

        string Message { get; }

        DateTime Date { get; }
    }
}
