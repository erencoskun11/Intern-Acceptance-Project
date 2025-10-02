using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class InternshipTaskConfiguration : IEntityTypeConfiguration<InternshipTask>
    {
        public void Configure(EntityTypeBuilder<InternshipTask> builder)
        {
            builder.ToTable("InternshipTasks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(t => t.Description)
                   .HasMaxLength(2000);

            builder.Property(t => t.AssignedAt)
                   .HasDefaultValueSql("now() at time zone 'utc'");

            builder.Property(t => t.SuccessPercent)
                   .HasColumnType("numeric(5,2)")
                   .HasDefaultValue(0m);

            builder.HasCheckConstraint("CK_InternshipTasks_SuccessRange", "\"SuccessPercent\" >= 0 AND \"SuccessPercent\" <= 100");

            builder.HasOne(t => t.InternshipApplication)
                   .WithMany(a => a.Tasks)
                   .HasForeignKey(t => t.InternshipApplicationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => t.InternshipApplicationId);
        }
    }
}
