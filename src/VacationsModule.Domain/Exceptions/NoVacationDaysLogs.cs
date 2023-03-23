namespace VacationsModule.Domain.Exceptions
{
    public class NoVacationDaysLogsException : Exception
    {
        public NoVacationDaysLogsException(string message) : base(message ?? "No vacation days logs found!")
        {

        }
    }
}