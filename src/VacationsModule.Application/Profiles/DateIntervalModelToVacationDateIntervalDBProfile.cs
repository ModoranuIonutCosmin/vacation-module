using AutoMapper;

namespace VacationsModule.Application.Profiles;

public class DateIntervalModelToVacationDateIntervalDBProfile : Profile
{
    public DateIntervalModelToVacationDateIntervalDBProfile()
    {
        CreateMap<Domain.Models.DateInterval, Domain.Entities.VacationRequestInterval>()
            .ReverseMap();
    }
}