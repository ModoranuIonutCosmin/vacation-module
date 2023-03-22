namespace VacationsModule.WebApi.ApiResponses;

public class GenericErrorResponse
{
    public string? Message { get; set; }
    
    public List<string> Issues { get; set; }
}