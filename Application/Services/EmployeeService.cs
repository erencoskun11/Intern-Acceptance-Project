using Application.Common;
using Application.DTOs.Employee;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Services
{
    public class EmployeeService : GenericService<Employee, EmployeeListDto, EmployeeCreateDto, EmployeeUpdateDto>, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISignalRService _signalRService;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper, IUnitOfWork uow, ISignalRService signalRService)
            : base(repository, mapper, uow)
        {
            _employeeRepository = repository;
            _signalRService = signalRService;
        }


        public override async Task<Response<int>> CreateAsync(EmployeeCreateDto dto)
        {
            // 1. ÇAPRAZ TABLO E-POSTA KONTROLÜ (Çalışanlar ve Stajyerler) [cite: 2025-12-19]
            var emailInEmployees = await _employeeRepository.AnyAsync(x => x.Email == dto.Email);
            var emailInInterns = await _uow.Interns.AnyAsync(x => x.Email == dto.Email);

            if (emailInEmployees || emailInInterns)
            {
                return Response<int>.Fail("Aynı gmaille stajyer veya çalışan kayıt edemezsiniz.", 400, true);
            }

            // 2. ÇAPRAZ TABLO TELEFON KONTROLÜ [cite: 2025-12-19]
            var phoneInEmployees = await _employeeRepository.AnyAsync(x => x.Phone == dto.Phone);
            var phoneInInterns = await _uow.Interns.AnyAsync(x => x.Phone == dto.Phone);

            if (phoneInEmployees || phoneInInterns)
            {
                return Response<int>.Fail("Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz.", 400, true);
            }

            // 3. TARİH KONTROLÜ
            if (dto.StartDate > DateTime.Now)
            {
                return Response<int>.Fail("İşe başlama tarihi gelecekte olamaz.", 400, true);
            }

            // 4. APPUSER (HESAP) NESNESİNİ OLUŞTURMA
            // Migration yasak olduğu için mevcut PasswordHash alanına şifreyi atıyoruz.
            var appUser = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PasswordHash = dto.Password, // DTO'dan gelen şifre [cite: 2025-12-20]
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            // 5. EMPLOYEE NESNESİNİ OLUŞTURMA VE BAĞLAMA
            var employee = _mapper.Map<Employee>(dto);
            employee.AppUser = appUser; // 1:1 ilişkiyi kurar, AppUserId'yi otomatik eşler.

            // 6. KAYIT VE COMMIT
            await _employeeRepository.AddAsync(employee);
            await _uow.CommitAsync();

            // 7. BİLDİRİMLER VE SİNYALLER
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.RefreshEntityListAsync("Employee");
            await _signalRService.SendNotificationAsync($"{dto.FirstName} {dto.LastName} için sistem girişi tanımlandı.", "success");

            return Response<int>.Success(employee.Id, 201);
        }

        public override async Task<Response<NoContent>> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            // 1. Çakışan E-Posta Kontrolü (Kendisi hariç başka birinde var mı?)
            var emailExists = await _employeeRepository.AnyAsync(x => x.Email == dto.Email && x.Id != id);
            if (emailExists)
            {
                return Response<NoContent>.Fail("Aynı gmaille stajyer veya çalışan kayıt edemezsiniz.", 400, true);
            }

            // 2. Çakışan Telefon Kontrolü (Kendisi hariç başka birinde var mı?)
            var phoneExists = await _employeeRepository.AnyAsync(x => x.Phone == dto.Phone && x.Id != id);
            if (phoneExists)
            {
                return Response<NoContent>.Fail("Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz.", 400, true);
            }

            // 3. Güncelleme İşlemi
            var response = await base.UpdateAsync(id, dto);

            if (response.IsSuccessful)
            {
                await _signalRService.RefreshEntityListAsync("Employee");
            }
            return response;
        }


        // Gerekli kütüphaneler ekli olmalı:
        // using Microsoft.EntityFrameworkCore;
        // using Npgsql;

        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            try
            {
                // 1. Kaydı bul
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                    return Response<NoContent>.Fail("Kayıt bulunamadı.", 404, true);

                // 2. Silmeyi dene
                _repository.Remove(entity);

                // Hata (Zimmet varsa) tam burada fırlar:
                await _uow.CommitAsync();

                // 3. Başarılıysa bildirimleri geç
                await _signalRService.RefreshEntityListAsync("Employee");
                await _signalRService.RefreshDashboardAsync();

                return Response<NoContent>.Success(204);
            }
            catch (DbUpdateException ex)
            {
                // 4. PostgreSQL "Bağlı Kayıt / Foreign Key" Hatası (Kod: 23503)
                // Eğer personelin zimmeti varsa bu hata düşer.
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
                {
                    return Response<NoContent>.Fail("Bu personeli silemezsiniz çünkü üzerinde zimmetli eşyalar bulunmaktadır. Lütfen önce zimmetleri kaldırın.", 400, true);
                }

                // Başka bir hataysa sistemin kendi hatasını fırlat
                throw;
            }
        }




        public async Task<IEnumerable<EmployeeListDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var query = _employeeRepository.GetByDepartmentId(departmentId);
            var employees = await query.ToListAsync();
            return _mapper.Map<IEnumerable<EmployeeListDto>>(employees);
        }

        public async Task<EmployeeListDto?> GetByEmailAsync(string email)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);
            return employee == null ? null : _mapper.Map<EmployeeListDto>(employee);
        }
    }
}