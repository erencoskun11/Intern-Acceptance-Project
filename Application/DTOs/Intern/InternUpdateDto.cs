using Domain.Enums; 
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Intern
{
    public class InternUpdateDto
    {
        public int Id { get; set; }

        // FullName silindi, yerine bunlar geldi
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        // Yeni iletişim bilgileri
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public string? StudentNumber { get; set; }

        public int ClassYear { get; set; }

        public int UniversityId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public DegreeType Degree { get; set; }
        public int WorkDaysPerWeek { get; set; }
    }
}