using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Department
{
    public class DepartmentCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
