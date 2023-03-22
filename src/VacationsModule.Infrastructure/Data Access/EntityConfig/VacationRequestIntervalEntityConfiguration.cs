using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class VacationRequestIntervalEntityConfiguration : IEntityTypeConfiguration<VacationRequestInterval>
{
    public void Configure(EntityTypeBuilder<VacationRequestInterval> vacationRequestIntervalConfig)
    {
        //Keys
        
        vacationRequestIntervalConfig
            .HasKey(vr => vr.Id);
        
        // Constraints

        vacationRequestIntervalConfig
            .Property(vr => vr.StartDate)
            .IsRequired();

        vacationRequestIntervalConfig
            .Property(vr => vr.EndDate)
            .IsRequired();
        
        //Index
        
        //Relations

        vacationRequestIntervalConfig
            .HasOne<VacationRequest>(vr => vr.VacationRequest)
            .WithMany(vr => vr.VacationIntervals);
    }
}