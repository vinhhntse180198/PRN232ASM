using AutoMapper;
using PRN232ASM.TrendService.Application.DTOs;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PublicationTrend, TrendResponse>();
        CreateMap<DashboardReport, ReportResponse>();
    }
}
