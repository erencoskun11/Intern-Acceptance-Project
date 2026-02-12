using Domain.Common; 

namespace Domain.Entities
{                                                                          
    public class AppUser : BaseEntity
    {

        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "User";


        public DateTime? LastLoginAt { get; set; }

        public Employee? Employee { get; set; }
    }
}