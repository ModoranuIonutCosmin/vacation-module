using System.ComponentModel.DataAnnotations;
using VacationsModule.Domain.Datamodels;

namespace VacationsModule.Application.DTOs;

public class GetVacationRequestsPaginatedRequest
{
    
    [Required]
    public int Page { get; init; }
    [Required]
    public int PageSize { get; init; }
    
    [Required]
    public VacationRequestStatus Status { get; init; }
    public Guid? EmployeeId { get; init; }
}