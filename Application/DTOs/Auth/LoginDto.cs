using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class LoginDto
    {
        public string UserName { get; set; } =null!;
        public string Password { get; set; } =null!;
    }
}
