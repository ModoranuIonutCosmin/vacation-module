namespace VacationsModule.Domain.Datamodels;

public enum VacationRequestStatus
{
    Pending = 1,
    RequiresClarification = 2,
    Canceled = 99,
    Rejected = 101,
    Approved = 100,
    ALL = 9999
}