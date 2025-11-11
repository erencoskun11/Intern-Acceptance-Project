using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Student
{
    public class StudentCreateDto
    {
        
        public string FullName { get; set; } = null!;

        public string? StudentNumber { get; set; }
        public int ClassYear { get; set; }
        public int UniversityId { get; set; }
    }
}
