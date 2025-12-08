using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = "Engineer"; 

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? AppUserId { get; set; } 
        public AppUser? AppUser { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    }
}
