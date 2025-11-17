using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Student
{
    public class StudentUpdateDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;

        public string? StudentNumber { get; set; }

        public int ClassYear { get; set; }

        public int UniversityId { get; set; }

    }
}
