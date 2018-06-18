using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Lykke.AlgoStore.Service.Logging.AzureRepositories.Entitites;
using Lykke.AlgoStore.Service.Logging.Core.Domain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.Service.Logging.AzureRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //To entities
            CreateMap<IUserLog, UserLogEntity>();

            ForAllMaps((map, cfg) =>
            {
                if (map.DestinationType.IsSubclassOf(typeof(TableEntity)))
                {
                    cfg.ForMember("ETag", opt => opt.Ignore());
                    cfg.ForMember("PartitionKey", opt => opt.Ignore());
                    cfg.ForMember("RowKey", opt => opt.Ignore());
                    cfg.ForMember("Timestamp", opt => opt.Ignore());
                }
            });

            //From entities (to custom DTOs if necessary)
        }
    }
}
