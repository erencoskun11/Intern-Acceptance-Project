using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.ToTable("Universities");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name)
                   .IsRequired()
                   .HasMaxLength(250);

          

            builder.HasData(
                new University { Id = 1, Name = "Example University" }
            );
        }
    }
}
