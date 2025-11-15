// InventoryItemCreateDto.cs
namespace Application.DTOs.InventoryItem
{
    public class InventoryItemCreateDto
    {
        public string ItemCode { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Location { get; set; }
    }
}