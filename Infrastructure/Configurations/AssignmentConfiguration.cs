using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments", t =>
            {
                t.HasCheckConstraint("CK_Assignments_Assignee",
                    "(\"InternId\" IS NOT NULL AND \"EmployeeId\" IS NULL) OR (\"InternId\" IS NULL AND \"EmployeeId\" IS NOT NULL)");
            });

            builder.HasKey(a => a.Id);

            builder.Property(a => a.AssignedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'");


            builder.Property(a => a.ActualReturnAt)
                    .HasColumnType("timestamptz");

            builder.HasOne(a => a.InventoryItem)
                    .WithMany(i => i.Assignments)
                    .HasForeignKey(a => a.InventoryItemId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Intern)
                    .WithMany(s => s.Assignments)
                    .HasForeignKey(a => a.InternId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Employee)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}