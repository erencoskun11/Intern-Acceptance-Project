// MaintenanceListDto.cs
namespace Application.DTOs.Maintenance
{
    public class MaintenanceListDto
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string InventoryItemCode { get; set; } = null!;
        public DateTime ReportedAt { get; set; }
        public DateTime? RepairedAt { get; set; }
        public string? Description { get; set; }
        public string? PerformedBy { get; set; }
        public decimal? Cost { get; set; }
    }
}