using Domain.Enums;
using System; // DateTime için gerekli

namespace Application.DTOs.InventoryItem
{
    public class InventoryItemCreateDto
    {
        public string ItemCode { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }

        // Eğer Entity'de Location yoksa bunu silebilirsin veya Entity'e eklemelisin.
        // public string? Location { get; set; } 

        public ItemStatus Status { get; set; }
        public string? Notes { get; set; }

        // --- EKSİKLER EKLENDİ ---
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
    }
}