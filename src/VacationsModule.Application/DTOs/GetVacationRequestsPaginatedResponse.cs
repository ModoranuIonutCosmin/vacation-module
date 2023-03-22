using VacationsModule.Domain.Datamodels;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.DTOs;

public class GetVacationRequestsPaginatedResponse
{
    public List<VacationRequestModel> VacationRequests { get; set; }
}