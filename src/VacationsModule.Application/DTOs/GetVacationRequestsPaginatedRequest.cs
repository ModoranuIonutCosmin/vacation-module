using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VacationsModule.Domain.Datamodels;

namespace VacationsModule.Application.DTOs;

public class GetVacationRequestsPaginatedRequest
{
    
    [Required]
    [DefaultValue(0)]
    public int Page { get; init; }
    [Required]
    [DefaultValue(10)]
    public int PageSize { get; init; }
    
    [Required]
    [DefaultValue(VacationRequestStatus.Pending)]
    public VacationRequestStatus Status { get; init; } = VacationRequestStatus.Pending;
    public Guid? EmployeeId { get; init; }
}