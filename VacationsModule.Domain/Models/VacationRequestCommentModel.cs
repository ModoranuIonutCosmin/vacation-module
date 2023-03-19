namespace VacationsModule.Domain.Models;

public class VacationRequestCommentModel
{
    public string Message { get; set; }
    public DateTimeOffset PostedAt { get; set; }
}