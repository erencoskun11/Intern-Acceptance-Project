using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class MentorConfiguration : IEntityTypeConfiguration<Mentor>
    {
        public void Configure(EntityTypeBuilder<Mentor> builder)
        {
            builder.ToTable("Mentors");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.FullName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(m => m.Email)
                   .HasMaxLength(200);

            builder.Property(m => m.Position)
                   .HasMaxLength(150);

            builder.HasOne(m => m.Department)
                   .WithMany(d => d.Mentors)
                   .HasForeignKey(m => m.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict); // departman silinirse mentor otomatik silinmesin
        }
    }
}
