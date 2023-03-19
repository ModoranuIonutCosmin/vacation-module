using AutoMapper;
using VacationsModule.Application.DTOs;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Application.Profiles;

public class VacationRequestDBToCreateVacationRequestResponseProfile : Profile
{
    public VacationRequestDBToCreateVacationRequestResponseProfile()
    {
        CreateMap<VacationRequest, CreateVacationRequestResponse>()
            .ForMember(dest => dest.VacationRequestId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RequestedDateIntervals, opt => opt.MapFrom(src => src.VacationIntervals))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}
