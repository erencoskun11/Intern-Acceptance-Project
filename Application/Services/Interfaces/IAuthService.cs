
using Application.Common;
using Application.DTOs.Auth;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Response<string>> LoginAsync(LoginDto loginDto);
        Task<Response<NoContent>> DeleteUserAsync(int id);
        Task<Response<AppUser>> GetMeAsync(string userName);

        Task<Response<AppUser>> CreateUserAsync(RegisterDto registerDto);
    }
}