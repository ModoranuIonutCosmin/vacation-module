namespace VacationsModule.Domain.Exceptions;

public class VacationRequestValidationException : Exception
{
    public VacationRequestValidationException()
    {
    }

    public VacationRequestValidationException(List<string> issues) : base(string.Join(Environment.NewLine, issues))
    {
        Issues = issues;
    }

    public List<string> Issues { get; set; }
}