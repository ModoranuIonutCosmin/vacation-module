using AutoMapper;
using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.Profiles;

public class VacationRequestToVacationRequestModelProfile : Profile
{
    
    public VacationRequestToVacationRequestModelProfile()
    {

        CreateMap<VacationRequestInterval, VacationRequestIntervalModel>().ReverseMap();
        CreateMap<VacationRequestComment, VacationRequestCommentModel>().ReverseMap();
        CreateMap<VacationRequest, VacationRequestModel>().ReverseMap();
    }
}