using Domain.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Intern : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? StudentNumber { get; set; }
        public int ClassYear { get; set; }

        public DegreeType Degree { get; set; } = DegreeType.Lisans;
        public DateTime StartDate { get; set; } = DateTime.UtcNow; 
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

        [Range(1, 5)]
        public int WorkDaysPerWeek { get; set; } = 5;

        public int UniversityId { get; set; }
        public University? University { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}