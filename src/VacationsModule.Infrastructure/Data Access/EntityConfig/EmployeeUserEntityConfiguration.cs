using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class EmployeeUserEntityConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        //Keys
        
        
        //Constraints
        
        builder
            .Property(e => e.Position)
            .HasMaxLength(50)
            .IsRequired();
                
        builder
            .Property(e => e.Department)
            .HasMaxLength(50)
            .IsRequired();

        // builder
        //     .Property(e => e.VacationDays)
        //     .IsRequired();



        //Index

        //Relations
    }
}