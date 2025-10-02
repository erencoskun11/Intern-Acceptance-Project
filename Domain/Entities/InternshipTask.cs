using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InternshipTask
    {
        public int Id { get; set; }

        public int InternshipApplicationId { get; set; }
        public InternshipApplication? InternshipApplication { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public decimal SuccessPercent { get; set; } = 0m;
    }
}
