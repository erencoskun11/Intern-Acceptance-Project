
using Domain.Common;
using Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class InternshipApplication : BaseEntity
    {
        // --- ESKİSİ: StudentId / Student ---
        // public int StudentId { get; set; }
        // public Student? Student { get; set; }

        // --- YENİSİ: InternId / Intern ---
        public int InternId { get; set; }
        public Intern? Intern { get; set; }
        // --------------------------------

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required]
        public InternshipRole Role { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsMandatory { get; set; } = false;
        public bool IsVolunteer { get; set; } = false;

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Column(TypeName = "numeric(5,2)")]
        [Range(0, 100)]
        public decimal TaskSuccessPercent { get; set; } = 0m;
    }
}