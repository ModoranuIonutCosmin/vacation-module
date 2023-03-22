namespace VacationsModule.Domain.Models;

public class VacationRequestModel
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<VacationRequestIntervalModel> VacationIntervals { get; set; }
    public List<VacationRequestCommentModel> Comments { get; set; }
}