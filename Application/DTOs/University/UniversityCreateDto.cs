using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.University
{
    public class UniversityCreateDto
    {
       
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
    }
}
