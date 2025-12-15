using Domain.Enums;
using System;

namespace Application.DTOs.InventoryItem
{
    public class InventoryItemUpdateDto
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = null!;
        public string? SerialNumber { get; set; }
        public string Category { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public ItemStatus Status { get; set; }
        public string? Notes { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}