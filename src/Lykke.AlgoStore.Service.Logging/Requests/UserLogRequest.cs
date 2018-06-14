using System;
using Lykke.AlgoStore.Service.Logging.Core.Domain;

namespace Lykke.AlgoStore.Service.Logging.Requests
{
    public class UserLogRequest : IUserLog
    {
        public string InstanceId { get; set; }
        public string Message { get; set; }
        private DateTime _date;

        public DateTime Date
        {
            get
            {
                if(_date == DateTime.MinValue || _date == DateTime.MaxValue)
                    return DateTime.UtcNow;

                return _date;
            }

            set => _date = value;
        }
    }
}
