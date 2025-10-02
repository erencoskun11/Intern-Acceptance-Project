using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InternshipApplication
    {
        public int Id { get; set; }

        // Ogrenci
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        // Basvurulan sirket departmani
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        [Required]
        public InternshipRole Role { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Zorunlu mu / istege bagli mi (ikisinin mantigi proje ihtiyacina gore)
        public bool IsMandatory { get; set; } = false;
        public bool IsVolunteer { get; set; } = false;

        // Atanan mentor (opsiyonel)
        public int? MentorId { get; set; }
        public Mentor? Mentor { get; set; }

        // Staj kabul sirasinda ogrenciye gonderilen tasklarin basari yuzdesi (0-100).
        // Precision veritabaninda numeric(5,2) seklinde ayarlanabilir.
        [Column(TypeName = "numeric(5,2)")]
        [Range(0, 100)]
        public decimal TaskSuccessPercent { get; set; } = 0m;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<InternshipTask>? Tasks { get; set; }
    }
}
