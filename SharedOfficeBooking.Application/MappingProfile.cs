using System.Text.Json;
using AutoMapper;
using SharedOfficeBooking.Application.Dtos;
using SharedOfficeBooking.Application.Helpers;
using SharedOfficeBooking.Domain.Entities;

namespace SharedOfficeBooking.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> Response DTO (deserialize floor plan)
        CreateMap<Workspace, WorkspaceResponseDto>()
            .ForMember(dest => dest.FloorPlan,
                opt => opt.MapFrom(src => FloorPlanHelper.DeserializeMatrix(src.FloorPlan)));

        // Create DTO -> Entity (serialize floor plan)
        CreateMap<WorkspaceCreateDto, Workspace>()
            .ForMember(dest => dest.FloorPlan,
                opt => opt.MapFrom(src => FloorPlanHelper.SerializeMatrix(src.FloorPlan)));
        
        // Update DTO -> Entity (serialize floor plan)
        CreateMap<WorkspaceUpdateDto, Workspace>()
            .ForMember(dest => dest.FloorPlan,
                opt => opt.MapFrom(src => FloorPlanHelper.SerializeMatrix(src.FloorPlan)));
        
        CreateMap<Booking, BookingResponseDto>().ReverseMap();
        CreateMap<BookingCreateDto, Booking>().ReverseMap();
    }
}