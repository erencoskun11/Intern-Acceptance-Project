using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        // Iliskiler
        public ICollection<Mentor>? Mentors { get; set; }
        public ICollection<InternshipApplication>? Applications { get; set; }
    }
}
