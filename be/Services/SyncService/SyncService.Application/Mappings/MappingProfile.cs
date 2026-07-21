using AutoMapper;
using SyncService.Application.DTOs;
using SyncService.Domain.Entities;

namespace SyncService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataSource, DataSourceDto>();
        CreateMap<SyncLog, SyncLogDto>()
            .ForMember(d => d.DataSourceName, o => o.MapFrom(s => s.DataSource.Name));
    }
}
