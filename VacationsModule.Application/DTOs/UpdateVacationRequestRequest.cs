using VacationsModule.Domain.Entities;
using VacationsModule.Domain.Models;

namespace VacationsModule.Application.DTOs;

public class UpdateVacationRequestRequest
{
    public Guid VacationRequestId { get; set; }
    public List<Domain.Models.DateInterval> RequestedDateIntervals { get; set; }
    
    public CommentModel? ExtraComment { get; set; }
    public string Description { get; set; }
}