using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService; 

        
        public AuthService(IUnitOfWork uow, ITokenService tokenService, IEmailService emailService)
        {
            _uow = uow;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<Response<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            // 1. Kullanıcı sorgusu
            var appUser = await _uow.AppUsers
                .Where(u => u.UserName == loginDto.UserName && u.PasswordHash == loginDto.Password)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync();

            if (appUser == null)
                return Response<LoginResponseDto>.Fail("Kullanıcı adı veya şifre yanlış", 401, true);

            // 2. Rol ve Token işlemleri
            var roles = new List<string> { appUser.Role };
            var tempEmployee = new Employee
            {
                Id = appUser.Id,
                FirstName = appUser.Employee?.FirstName ?? appUser.UserName,
                LastName = appUser.Employee?.LastName ?? "",
                Email = appUser.Email
            };

            var token = _tokenService.CreateToken(tempEmployee, roles);

            // 3. ⭐ NET YAPI: LoginResponseDto tipinde veri dönüyoruz
            return Response<LoginResponseDto>.Success(new LoginResponseDto
            {
                Token = token,
                Role = appUser.Role,
                Email = appUser.Email
            }, 200);
        }
        public async Task<Response<AppUser>> CreateUserAsync(RegisterDto registerDto)
        {
            // 1. Email Kontrolü: Bu email daha önce sisteme kayıt edilmiş mi?
            var exists = await _uow.AppUsers.AnyAsync(x => x.Email == registerDto.Email);
            if (exists)
                return Response<AppUser>.Fail("Bu email zaten kayıtlı.", 400, true);

            // 2. ÖZEL ADMİN KONTROLÜ: Belirtilen mailler her zaman Admin rolüyle kaydedilir
            var adminEmails = new List<string>
    {
        "eren1coskun11@gmail.com",
        "meryemdinc45@gmail.com"
    };

            // Eğer mail listedeyse "Admin", değilse RegisterDto'dan gelen rolü ata
            string finalRole = adminEmails.Contains(registerDto.Email) ? "Admin" : registerDto.Role;

            // 3. Yeni Kullanıcı Nesnesinin Oluşturulması
            var newUser = new AppUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email, // Kullanıcı adı olarak mail adresi kullanılır
                Role = finalRole,             // Yukarıda belirlenen güvenli rol atanır
                PasswordHash = registerDto.Password, // Şifre (Gerçek projede Hashlenmelidir)
            };

            // 4. Veritabanına Kayıt İşlemi
            await _uow.AppUsers.AddAsync(newUser);
            await _uow.CommitAsync();

            // 5. Bilgilendirme Maili Gönderme (Try-Catch ile korunmuştur)
            try
            {
                string mailKonu = "Stajyer Takip Sistemi - Hoşgeldiniz";
                string mailIcerik = $@"
            <h3>Merhaba,</h3>
            <p>Sisteme kaydınız başarıyla oluşturulmuştur.</p>
            <p><strong>Kullanıcı Adı:</strong> {newUser.Email}</p>
            <p><strong>Şifre:</strong> {registerDto.Password}</p>
            <p><strong>Yetki Seviyesi:</strong> {finalRole}</p>
            <br>
            <p>İyi çalışmalar dileriz.</p>";

                await _emailService.SendEmailAsync(newUser.Email, mailKonu, mailIcerik);
            }
            catch
            {
                // Mail gitmese bile kullanıcı kaydı tamamlandığı için işlem durdurulmaz
            }

            return Response<AppUser>.Success(newUser, 201);
        }
        public async Task<Response<NoContent>> DeleteUserAsync(int id)
        {
            var user = await _uow.AppUsers.GetByIdAsync(id);
            if (user == null) return Response<NoContent>.Fail("Kullanıcı bulunamadı", 404, true);

            _uow.AppUsers.Remove(user);
            await _uow.CommitAsync();

            return Response<NoContent>.Success(204);
        }

        public async Task<Response<AppUser>> GetMeAsync(string userName)
        {
            var appUser = await _uow.AppUsers
                .Where(u => u.UserName == userName)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync();

            if (appUser == null) return Response<AppUser>.Fail("Kullanıcı bulunamadı", 404, true);

            return Response<AppUser>.Success(appUser, 200);
        }
    }
}