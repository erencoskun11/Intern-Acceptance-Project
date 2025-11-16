// InventoryItemListDto.cs
namespace Application.DTOs.InventoryItem
{
    public class InventoryItemListDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string Status { get; set; } = null!;
        public string? Location { get; set; }
    }
}