using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Mentor
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string? Email { get; set; }
        public string? Position { get; set; }

        // Hangi departmanda calisiyor
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Mentorun atandigi basvurular
        public ICollection<InternshipApplication>? AssignedApplications { get; set; }
    }
}
