using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [MaxLength(50)]
        public string? StudentNumber { get; set; } // ogrenci no (opsiyonel ama uniq olabilir)

        [Required]
        public int ClassYear { get; set; } // sinif (3,4 vb.)

        // Universite ve advisor
        public int UniversityId { get; set; }
        public University? University { get; set; }

        public ICollection<InternshipApplication>? Applications { get; set; }
    }
}
