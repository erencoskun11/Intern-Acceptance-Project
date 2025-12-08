using Domain.Common;
using Domain.Enums;

namespace Domain.Entities
{
    public class InventoryItem : BaseEntity
    {
        public string ItemCode { get; set; } = null!;
        public string? SerialNumber { get; set; }  
        public string Category { get; set; } = null!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public ItemStatus Status { get; set; } = ItemStatus.Available;
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public string? Notes { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
    }

  
}
