using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(d => d.Name).IsUnique();

            // Seed departmanlar
            builder.HasData(
                new Department { Id = 1, Name = "Frontend" },
                new Department { Id = 2, Name = "Backend" },
                new Department { Id = 3, Name = "Mobile" },
                new Department { Id = 4, Name = "Database" },
                new Department { Id = 5, Name = "Fullstack" }
            );
        }
    }
}
