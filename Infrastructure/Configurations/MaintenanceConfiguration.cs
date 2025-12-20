using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class MaintenanceConfiguration : IEntityTypeConfiguration<Maintenance>
    {
        public void Configure(EntityTypeBuilder<Maintenance> builder)
        {
            builder.ToTable("Maintenances");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.ReportedAt)
                   .HasDefaultValueSql("now() at time zone 'utc'");

            builder.Property(m => m.RepairedAt)
                   .HasColumnType("timestamptz");

            builder.Property(m => m.Description).HasMaxLength(2000);

            builder.HasOne(m => m.InventoryItem)
                   .WithMany(i => i.Maintenances)
                   .HasForeignKey(m => m.InventoryItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.InventoryItemId);
        }
    }
}
