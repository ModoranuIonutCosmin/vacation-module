using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        //Keys
        
        builder
            .HasKey(au => au.Id);
        
        //Constraints
        
        /// Discriminator
        builder
            .Property("Discriminator")
            .HasMaxLength(50)
            .IsRequired();
        
        builder
            .Property(au => au.UserName)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(au => au.NormalizedUserName)
            .HasMaxLength(50);
            // .IsRequired();
        
        builder
            .Property(au => au.FirstName)
            .HasMaxLength(50)
            .IsRequired();
        
        
        builder
            .Property(au => au.LastName)
            .HasMaxLength(50)
            .IsRequired();
        
        builder
            .Property(au => au.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(au => au.NormalizedEmail)
            .HasMaxLength(50);
            // .IsRequired();



        builder
            .Property(au => au.PhoneNumber)
            .HasMaxLength(50);

        //Index

        //Relations
    }
}