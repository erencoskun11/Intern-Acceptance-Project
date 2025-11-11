namespace Application.DTOs.Maintenance
{
    public class MaintenanceUpdateDto
    {
        public DateTime? RepairedAt { get; set; }
        public string? Notes { get; set; }
        public decimal? Cost { get; set; }
    }
}