using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Maintenance : BaseEntity
    {

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public DateTime ReportedAt { get; set; }
        public DateTime? RepairedAt { get; set; }

        public string? Description { get; set; }
        public string? PerformedBy { get; set; }  
        public decimal? Cost { get; set; }
    }
}
