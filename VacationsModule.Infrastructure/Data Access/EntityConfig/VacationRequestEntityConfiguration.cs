using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class VacationRequestEntityConfiguration : IEntityTypeConfiguration<VacationRequest>
{
    public void Configure(EntityTypeBuilder<VacationRequest> vacationRequestConfig)
    {
        //Keys
        
        vacationRequestConfig
            .HasKey(vr => vr.Id);
        
        // Constraints

        vacationRequestConfig
            .Property<string>(vr => vr.Description)
            .HasMaxLength(3999)
            .IsRequired();

        vacationRequestConfig
            .Property(vr => vr.Status)
            .IsRequired();

        vacationRequestConfig
            .Property(vr => vr.CreatedAt)
            .IsRequired();
        
        //Index
        
        vacationRequestConfig
            .HasIndex(vr => vr.Status);
        
        
        //Relations
        
        vacationRequestConfig
            .HasMany(o => o.VacationIntervals)
            .WithOne();

        vacationRequestConfig
            .HasMany(vr => vr.Comments)
            .WithOne(
                c => c.VacationRequest
                )
            .OnDelete(DeleteBehavior.Cascade);

        vacationRequestConfig
            .HasOne<Employee>(vr => vr.Employee)
            .WithMany(e => e.VacationRequests)
            .OnDelete(DeleteBehavior.NoAction);
        
        // 2 Path-uri de cascade delete: delete User -> delete Requests -> delete RequestComments
        // Sau: delete Request -> delete RequestsComments
        // Reminder: Nu se sterg useri oricum deci NoAction pe user -> requests

        // vacationRequestConfig
        //     .HasOne<Manager>(vr => vr.Approver);
    }
}