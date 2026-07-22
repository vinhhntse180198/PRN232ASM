using AutoMapper;
using NotificationService.Application.DTOs;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Notification, NotificationDto>();
    }
}
