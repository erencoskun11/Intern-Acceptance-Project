using Domain.Entities;
using System.Collections.Generic;

namespace Application.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(Employee employee, IList<string> roles);

        string CreateToken(Intern student, IList<string> roles);
    }
}
