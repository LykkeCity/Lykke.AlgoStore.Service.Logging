using AutoMapper;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Requests;

namespace Lykke.AlgoStore.Service.Logging
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IUserLog, UserLogRequest>();
        }
    }
}
