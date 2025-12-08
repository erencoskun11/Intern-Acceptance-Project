using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Assignment : BaseEntity
    {

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public int? InternId { get; set; }
        public Intern? Intern { get; set; }

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? ActualReturnAt { get; set; }
        public string? Notes { get; set; }
    }
}
