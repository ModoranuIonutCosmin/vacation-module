using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacationsModule.Domain.Entities;

namespace VacationsModule.Infrastructure.Data_Access.EntityConfig;

public class VacationRequestCommentEntityConfiguration : IEntityTypeConfiguration<VacationRequestComment>
{
    public void Configure(EntityTypeBuilder<VacationRequestComment> builder)
    {
        //Keys
        
        builder
            .HasKey(vrc => vrc.Id);
        
        //Constraints
        
        builder
            .Property(vrc => vrc.Message)
            .HasMaxLength(3999)
            .IsRequired();

        builder.Property(vrc => vrc.PostedAt)
            .IsRequired();
        
        //Index
        
        //Relations

        builder
            .HasOne<ApplicationUser>(vrc => vrc.Author)
            .WithMany();
            // .HasForeignKey(vrc => vrc.AuthorId);

        // builder
        //     .HasOne<VacationRequest>(vrc => vrc.VacationRequest)
        //     .WithMany(vrc => vrc.Comments)
        //     .OnDelete(DeleteBehavior.Cascade);
            // .HasForeignKey(vrc => vrc.VacationRequestId);
    }
}