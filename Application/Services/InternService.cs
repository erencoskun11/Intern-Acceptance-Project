using Application.Common;
using Application.DTOs.Intern;
using Application.Interfaces;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InternService : GenericService<Intern, InternListDto, InternCreateDto, InternUpdateDto>, IInternService
    {
        private readonly IInternRepository _internRepository;
        private readonly ISignalRService _signalRService;

        public InternService(IInternRepository repository, IMapper mapper, IUnitOfWork uow,ISignalRService signalRService)
            : base(repository, mapper, uow)
        {
            _internRepository = repository;
            _signalRService = signalRService;
        }

        public override async Task<Response<int>> CreateAsync(InternCreateDto dto)
        {
            // 1. E-Posta Kontrolü (LINQ)
            var emailExists = await _internRepository.AnyAsync(x => x.Email == dto.Email);
            if (emailExists)
            {
                return Response<int>.Fail("Aynı gmaille stajyer veya çalışan kayıt edemezsiniz.", 400, true);
            }

            // 2. Telefon Kontrolü (LINQ) [cite: 2025-12-19]
            var phoneExists = await _internRepository.AnyAsync(x => x.Phone == dto.Phone);
            if (phoneExists)
            {
                return Response<int>.Fail("Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz.", 400, true);
            }

            // 3. Kayıt İşlemi
            var intern = _mapper.Map<Intern>(dto);
            await _internRepository.AddAsync(intern);
            await _uow.CommitAsync();

            // 4. SignalR Tetiklemeleri
            await _signalRService.RefreshEntityListAsync("Intern");
            await _signalRService.RefreshDashboardAsync();
            await _signalRService.SendNotificationAsync($"{intern.FirstName} {intern.LastName} isimli stajyer eklendi.", "success");

            return Response<int>.Success(intern.Id, 201);
        }
        public async Task<Response<IEnumerable<InternListDto>>> GetAllWithDetailsAsync()
        {
            var query = _internRepository.GetAllWithDetails();
            var interns = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<InternListDto>>(interns);

            return Response<IEnumerable<InternListDto>>.Success(dtos, 200);
        }




        public override async Task<Response<NoContent>> UpdateAsync(int id, InternUpdateDto dto)
        {
            // 1. DİĞER STAJYERLERDE VAR MI? (Kendi ID'si hariç kontrol eder) [cite: 2025-12-19]
            var emailInInterns = await _uow.Interns.AnyAsync(x => x.Email == dto.Email && x.Id != id);
            var phoneInInterns = await _uow.Interns.AnyAsync(x => x.Phone == dto.Phone && x.Id != id);

            // 2. ÇALIŞANLAR TABLOSUNDA VAR MI? (Cross-table kontrolü) [cite: 2025-12-19]
            var emailInEmployees = await _uow.Employees.AnyAsync(x => x.Email == dto.Email);
            var phoneInEmployees = await _uow.Employees.AnyAsync(x => x.Phone == dto.Phone);

            // --- HATA KONTROLLERİ ---
            if (emailInInterns || emailInEmployees)
            {
                return Response<NoContent>.Fail("Aynı gmaille stajyer veya çalışan kayıt edemezsiniz.", 400, true);
            }

            if (phoneInInterns || phoneInEmployees)
            {
                return Response<NoContent>.Fail("Aynı telefon numarası ile stajyer veya çalışan kabul edemezsiniz.", 400, true);
            }

            // 3. Validasyonlar geçildiyse ana güncelleme işlemini çağır [cite: 2025-12-18]
            var response = await base.UpdateAsync(id, dto);

            if (response.IsSuccessful)
            {
                // UI tarafını anlık güncellemek için SignalR tetiklemeleri
                await _signalRService.RefreshEntityListAsync("Intern");
                await _signalRService.RefreshDashboardAsync();
            }

            return response;
        }
        public override async Task<Response<NoContent>> DeleteAsync(int id)
        {
            var response = await base.DeleteAsync(id);

            if(response.IsSuccessful)
            {
                await _signalRService.RefreshDashboardAsync();
                await _signalRService.RefreshEntityListAsync("Intern");
                await _signalRService.SendNotificationAsync("Stajyer silindi.", "warning");
            }
            return response;
        }

        }
}