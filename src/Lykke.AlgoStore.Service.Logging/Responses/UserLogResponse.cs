using System;
using Lykke.AlgoStore.Service.Logging.Core.Domain;

namespace Lykke.AlgoStore.Service.Logging.Responses
{
    public class UserLogResponse : IUserLog
    {
        public string InstanceId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
