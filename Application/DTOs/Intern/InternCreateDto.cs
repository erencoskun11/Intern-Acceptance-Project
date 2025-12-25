using Domain.Enums;

namespace Application.DTOs.Intern
{
    public class InternCreateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? StudentNumber { get; set; }
        public int ClassYear { get; set; }

        public DegreeType Degree { get; set; }  // Türkçe Enum Tipi
        public int WorkDaysPerWeek { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public int DepartmentId { get; set; }
        public int UniversityId { get; set; }

        public string? Password { get; set; } // Kullanıcının giriş şifresi
    }
}