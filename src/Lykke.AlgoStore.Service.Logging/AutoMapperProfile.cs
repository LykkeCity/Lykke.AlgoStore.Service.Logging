using AutoMapper;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Lykke.AlgoStore.Service.Logging.Requests;
using Lykke.AlgoStore.Service.Logging.Responses;

namespace Lykke.AlgoStore.Service.Logging
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IUserLog, UserLogRequest>();

            CreateMap<IUserLog, UserLogResponse>();
        }
    }
}
