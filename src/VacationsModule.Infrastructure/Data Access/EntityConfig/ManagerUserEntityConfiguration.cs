using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class ManagerUserEntityConfiguration : IEntityTypeConfiguration<Manager>
{
    public void Configure(EntityTypeBuilder<Manager> builder)
    {
        //Keys
        
        
        //Constraints
        
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