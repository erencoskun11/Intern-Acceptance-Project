using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class InternshipApplicationConfiguration : IEntityTypeConfiguration<InternshipApplication>
    {
        public void Configure(EntityTypeBuilder<InternshipApplication> builder)
        {
            builder.ToTable("InternshipApplications");

            builder.HasKey(x => x.Id);

            // Enum -> string olarak sakla (okunabilir)
            builder.Property(x => x.Role)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.StartDate).IsRequired();
            builder.Property(x => x.EndDate).IsRequired();

            // Task success precision
            builder.Property(x => x.TaskSuccessPercent)
                   .HasColumnType("numeric(5,2)")
                   .HasDefaultValue(0m);

            // Check constraint: task success 0-100
            builder.HasCheckConstraint("CK_InternshipApplications_TaskSuccessRange", "\"TaskSuccessPercent\" >= 0 AND \"TaskSuccessPercent\" <= 100");

            // Relations
            builder.HasOne(x => x.Intern)
                   .WithMany(s => s.Applications)
                   .HasForeignKey(x => x.InternId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Department)
                   .WithMany(d => d.Applications)
                   .HasForeignKey(x => x.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Employee)
                   .WithMany(m => m.AssignedApplications)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Add an index to frequently queried columns
            builder.HasIndex(x => x.InternId);
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => new { x.InternId, x.Role });

            // Shadow property for concurrency / rowversion (opsiyonel)
            builder.Property<byte[]>("RowVersion").IsRowVersion();

            // Default CreatedAt
            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
