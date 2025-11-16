// InventoryItemUpdateDto.cs
namespace Application.DTOs.InventoryItem
{
    public class InventoryItemUpdateDto
    {
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}