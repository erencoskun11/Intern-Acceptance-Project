using Application.Common;
using Application.DTOs.Assignment;
using Application.DTOs.Maintenance;
using Application.Interfaces; 
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class AssignmentService : GenericService<Assignment, AssignmentListDto, AssignmentCreateDto, AssignmentUpdateDto>, IAssignmentService
    {
        private readonly ISignalRService _signalRService;
        private readonly IEmailService _emailService; 

        public AssignmentService(
            IUnitOfWork uow,
            IMapper mapper,
            ISignalRService signalRService,
            IEmailService emailService) 
            : base(uow.Assignments, mapper, uow)
        {
            _signalRService = signalRService;
            _emailService = emailService;
        }

        public override async Task<Response<int>> CreateAsync(AssignmentCreateDto dto)
        {
            var item = await _uow.InventoryItems.GetByIdAsync(dto.InventoryItemId);
            if (item == null) return Response<int>.Fail("Seçilen ürün bulunamadı.", 404, true);

            if (item.Status != ItemStatus.Available)
                return Response<int>.Fail($"Bu ürün zimmetlenemez. Durum: {item.Status}", 400, true);


            var activeAssignments = await _uow.Assignments.GetActiveAssignmentsByPerson(dto.InternId, dto.EmployeeId).ToListAsync();
            if (activeAssignments.Any(a => a.InventoryItem.Category == item.Category))
            {
                return Response<int>.Fail($"Bu kişiye zaten '{item.Category}' kategorisinde bir ürün zimmetlenmiş.", 400, true);
            }

            // 3. Zimmet Kaydı
            var assignment = _mapper.Map<Assignment>(dto);
            await _uow.Assignments.AddAsync(assignment);

            // 4. Ürün Durumu Güncelleme
            item.Status = ItemStatus.Assigned;
            _uow.InventoryItems.Update(item);

            await _uow.CommitAsync();

            // --- SIGNALR TETİKLEMELERİ ---
            await _signalRService.SendNotificationAsync("Yeni bir zimmet yapıldı!", "success");
            await _signalRService.RefreshEntityListAsync("Assignment");
            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshDashboardAsync();

            // --- MAİL GÖNDERME İŞLEMİ (YENİ) ---
            // Mail işlemi başarısız olsa bile zimmet kaydı bozulmasın diye ayrı bir task olarak veya try-catch içinde çalıştırılabilir.
            // Burada 'await' ile bekletiyoruz ki mail gittiğinden emin olalım.
            try
            {
                await SendAssignmentEmail(dto, item);
            }
            catch (Exception ex)
            {
                // Mail hatası olursa logla ama işlemi durdurma
                Console.WriteLine($"Mail Gönderilemedi: {ex.Message}");
            }

            return Response<int>.Success(assignment.Id, 201);
        }

        // --- YARDIMCI MAİL METODU ---
        private async Task SendAssignmentEmail(AssignmentCreateDto dto, InventoryItem item)
        {
            string toEmail = null;
            string recipientName = null;

            // Kime zimmetlendiğini bul
            if (dto.EmployeeId.HasValue)
            {
                var employee = await _uow.Employees.GetByIdAsync(dto.EmployeeId.Value);
                if (employee != null)
                {
                    toEmail = employee.Email;
                    recipientName = $"{employee.FirstName} {employee.LastName}";
                }
            }
            else if (dto.InternId.HasValue)
            {
                var intern = await _uow.Interns.GetByIdAsync(dto.InternId.Value);
                if (intern != null)
                {
                    toEmail = intern.Email;
                    recipientName = $"{intern.FirstName} {intern.LastName}";
                }
            }

            // Eğer mail adresi varsa gönder
            if (!string.IsNullOrEmpty(toEmail))
            {
                string subject = "📦 Yeni Zimmet Bilgilendirmesi";
                string body = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                        <h2 style='color: #2c3e50;'>Sayın {recipientName},</h2>
                        <p>Adınıza yeni bir envanter zimmetlenmiştir. Detaylar aşağıdadır:</p>
                        
                        <table style='width: 100%; border-collapse: collapse; margin-top: 10px;'>
                            <tr style='background-color: #f2f2f2;'>
                                <td style='padding: 10px; border: 1px solid #ddd;'><b>Ürün Kodu</b></td>
                                <td style='padding: 10px; border: 1px solid #ddd;'>{item.ItemCode}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border: 1px solid #ddd;'><b>Kategori</b></td>
                                <td style='padding: 10px; border: 1px solid #ddd;'>{item.Category}</td>
                            </tr>
                            <tr style='background-color: #f2f2f2;'>
                                <td style='padding: 10px; border: 1px solid #ddd;'><b>Marka / Model</b></td>
                                <td style='padding: 10px; border: 1px solid #ddd;'>{item.Brand} / {item.Model}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border: 1px solid #ddd;'><b>Seri No</b></td>
                                <td style='padding: 10px; border: 1px solid #ddd;'>{item.SerialNumber}</td>
                            </tr>
                        </table>

                        <p style='margin-top: 20px;'>Zimmet tutanağını sistem üzerinden görüntüleyebilirsiniz.</p>
                        <p style='color: #7f8c8d; font-size: 12px;'>Bu e-posta otomatik olarak gönderilmiştir.</p>
                    </div>
                ";

                await _emailService.SendEmailAsync(toEmail, subject, body);
            }
        }

        // --- DİĞER METOTLAR (DEĞİŞMEDİ) ---
        public override async Task<Response<IEnumerable<AssignmentListDto>>> GetAllAsync()
        {
            var query = _uow.Assignments.GetAll(
                a => a.InventoryItem,
                a => a.Intern,
                a => a.Employee
            );
            var list = await query.ToListAsync();
            var dtos = _mapper.Map<IEnumerable<AssignmentListDto>>(list);
            return Response<IEnumerable<AssignmentListDto>>.Success(dtos ?? new List<AssignmentListDto>(), 200);
        }

        public async Task<Response<NoContent>> ReturnAsync(AssignmentReturnDto dto)
        {
            var assignment = await _uow.Assignments.GetByIdAsync(dto.AssignmentId, a => a.InventoryItem);
            if (assignment == null) return Response<NoContent>.Fail("Zimmet kaydı bulunamadı.", 404, true);

            if (assignment.ActualReturnAt.HasValue)
                return Response<NoContent>.Fail("Bu ürün zaten iade edilmiş.", 400, true);

            assignment.ActualReturnAt = dto.ActualReturnAt ?? DateTime.UtcNow;
            _uow.Assignments.Update(assignment);

            if (assignment.InventoryItem != null)
            {
                assignment.InventoryItem.Status = ItemStatus.Available;
                _uow.InventoryItems.Update(assignment.InventoryItem);
            }

            await _uow.CommitAsync();

            await _signalRService.SendNotificationAsync("Ürün iade alındı.", "info");
            await _signalRService.RefreshEntityListAsync("Assignment");
            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshDashboardAsync();

            return Response<NoContent>.Success(204);
        }

        public async Task<Response<int>> CreateMaintenanceAsync(MaintenanceCreateDto dto)
        {
            var item = await _uow.InventoryItems.GetByIdAsync(dto.InventoryItemId);
            if (item == null) return Response<int>.Fail("Ürün bulunamadı.", 404, true);

            if (item.Status == ItemStatus.Assigned) return Response<int>.Fail("Ürün şu an zimmetli.", 400, true);
            if (item.Status == ItemStatus.Maintenance) return Response<int>.Fail("Ürün zaten bakımda.", 400, true);

            var maintenance = _mapper.Map<Maintenance>(dto);
            await _uow.Maintenances.AddAsync(maintenance);

            item.Status = ItemStatus.Maintenance;
            _uow.InventoryItems.Update(item);

            await _uow.CommitAsync();

            await _signalRService.RefreshEntityListAsync("Inventory");
            await _signalRService.RefreshEntityListAsync("Maintenance");

            return Response<int>.Success(maintenance.Id, 201);
        }

        public async Task<Response<IEnumerable<AssignmentListDto>>> GetByEmployeeIdAsync(int employeeId)
        {
            var list = await _uow.Assignments.GetByEmployeeId(employeeId).ToListAsync();
            var dtos = _mapper.Map<IEnumerable<AssignmentListDto>>(list);
            return Response<IEnumerable<AssignmentListDto>>.Success(dtos, 200);
        }

        public async Task<Response<IEnumerable<AssignmentListDto>>> GetByInternIdAsync(int internId)
        {
            var list = await _uow.Assignments.GetByInternId(internId).ToListAsync();
            var dtos = _mapper.Map<IEnumerable<AssignmentListDto>>(list);
            return Response<IEnumerable<AssignmentListDto>>.Success(dtos, 200);
        }
    }
}