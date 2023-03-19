using VacationsModule.Domain.DomainServices.Interfaces;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Domain.DomainServices.Validators;

public class VacationDateIntervalsValidator : IVacationDateIntervalsValidator
{
    public (bool IsValidated, List<string> Issues) Validate(List<VacationRequestInterval> dateIntervals)
    {
        var issues = new List<string>();
        
        if (dateIntervals == null || dateIntervals.Count == 0)
        {
            return (false, new List<string> {"Date intervals are empty"});
        }
        
        if (!DateIntervalsAreNotInThePast(dateIntervals))
        {
            issues.Add("Date intervals are in the past");
        }
        
        if (!DateIntervalsDoNotOverlap(dateIntervals))
        {
            issues.Add("Date intervals overlap");
        }


        return (IsValidated: !dateIntervals.Any(), issues);
    }

    private bool DateIntervalsAreNotInThePast(List<VacationRequestInterval> dateIntervals)
    {
        //check if any of the intervals is in the past
        
        return dateIntervals.All(x => x.StartDate >= DateTimeOffset.UtcNow);
    }


    private bool DateIntervalsDoNotOverlap(List<VacationRequestInterval> dateIntervals)
    {
        if (dateIntervals.Count == 1)
        {
            return true;
        }
        
        for (int interval1Index = 0; interval1Index < dateIntervals.Count; interval1Index++)
        {
            for (int interval2Index = 0; interval2Index < dateIntervals.Count; interval2Index++)
            {
                if (interval1Index == interval2Index)
                {
                    continue;
                }
                
                if (dateIntervals[interval1Index].StartDate >= dateIntervals[interval2Index].EndDate ||
                    dateIntervals[interval1Index].EndDate <= dateIntervals[interval2Index].StartDate)
                {
                    continue;
                }
                
                return false;
            }
        }
        
        return true;
        
    }
}