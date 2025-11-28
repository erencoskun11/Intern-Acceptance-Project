using Application.DTOs.Assignment;
using Application.DTOs.Maintenance;
using Application.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// Kendi Exception sınıfların yoksa System.Exception kullanabilirsin
using Application.Exceptions;

namespace Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IGenericRepository<InventoryItem> _inventoryItemRepository;
        private readonly IGenericRepository<Maintenance> _maintenanceRepository;
        private readonly IMapper _mapper;

        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            IGenericRepository<InventoryItem> inventoryItemRepository,
            IGenericRepository<Maintenance> maintenanceRepository,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _maintenanceRepository = maintenanceRepository;
            _mapper = mapper;
        }

        // --- 1. ZİMMET OLUŞTURMA (Assign) ---
        public async Task<int> AssignAsync(AssignmentCreateDto dto)
        {
            // Validasyon: Sadece bir kişi seçilmeli
            if ((dto.InternId.HasValue && dto.EmployeeId.HasValue) ||
                (!dto.InternId.HasValue && !dto.EmployeeId.HasValue))
            {
                throw new ArgumentException("Zimmet işlemi için sadece bir kişi (Stajyer veya Personel) seçilmelidir.");
            }

            // Ürün Kontrolü
            var item = await _inventoryItemRepository.GetByIdAsync(dto.InventoryItemId);
            if (item == null)
                throw new KeyNotFoundException("Seçilen ürün bulunamadı.");

            if (item.Status != ItemStatus.Available)
                throw new InvalidOperationException($"Bu ürün zimmetlenemez. Şu anki durumu: {item.Status}");

            // KATEGORİ KONTROLÜ (Aynı kategoriden 2 ürün verilemez)
            var activeAssignments = await _assignmentRepository.GetActiveAssignmentsByPersonAsync(dto.InternId, dto.EmployeeId);

            bool hasSameCategory = activeAssignments.Any(a => a.InventoryItem.Category == item.Category);
            if (hasSameCategory)
            {
                throw new InvalidOperationException($"Bu kişiye zaten '{item.Category}' kategorisinde bir ürün zimmetlenmiş.");
            }

            // Kayıt İşlemi
            var assignment = _mapper.Map<Assignment>(dto);
            await _assignmentRepository.AddAsync(assignment);

            // Ürün Durumunu Güncelle
            item.Status = ItemStatus.Assigned;
            await _inventoryItemRepository.UpdateAsync(item);

            // UnitOfWork yapısı yoksa manuel SaveChanges gerekebilir
            // await _assignmentRepository.SaveChangesAsync(); 

            return assignment.Id;
        }

        // --- 2. İADE (Return) ---
        public async Task ReturnAsync(AssignmentReturnDto dto)
        {
            // Include ile ürünü de çekiyoruz ki durumunu güncelleyelim
            var assignment = await _assignmentRepository.GetByIdAsync(dto.AssignmentId, a => a.InventoryItem);

            if (assignment == null)
                throw new KeyNotFoundException("Zimmet kaydı bulunamadı.");

            if (assignment.ActualReturnAt.HasValue)
                throw new InvalidOperationException("Bu ürün zaten iade edilmiş.");

            // İade Tarihini İşle
            assignment.ActualReturnAt = dto.ActualReturnAt ?? DateTime.UtcNow;
            await _assignmentRepository.UpdateAsync(assignment);

            // Ürün Durumunu Müsait Yap
            if (assignment.InventoryItem != null)
            {
                assignment.InventoryItem.Status = ItemStatus.Available;
                await _inventoryItemRepository.UpdateAsync(assignment.InventoryItem);
            }
        }

        // --- 3. UPDATE (Düzeltme) ---
        public async Task UpdateAsync(int id, AssignmentUpdateDto dto)
        {
            var entity = await _assignmentRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Zimmet kaydı bulunamadı.");

            // Notları veya tarihleri güncelle
            entity.Notes = dto.Notes;
            if (dto.ActualReturnAt.HasValue) entity.ActualReturnAt = dto.ActualReturnAt;
            // Diğer alanlar (InventoryItemId vs.) genelde update edilmez, edilirse logic karışır.

            await _assignmentRepository.UpdateAsync(entity);
        }

        // --- 4. DELETE (Silme) ---
        public async Task DeleteAsync(int id)
        {
            var entity = await _assignmentRepository.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Zimmet kaydı bulunamadı.");

            // Kural: İade edilmemiş (Aktif) zimmet silinemez
            if (entity.ActualReturnAt == null)
                throw new InvalidOperationException("Aktif bir zimmet kaydı silinemez. Önce iade almalısınız.");

            await _assignmentRepository.DeleteAsync(id);
        }

        // --- GET METOTLARI ---
        public async Task<IEnumerable<AssignmentListDto>> GetAllAsync()
        {
            var list = await _assignmentRepository.GetAllAsync(a => a.InventoryItem, a => a.Intern, a => a.Employee);
            return _mapper.Map<IEnumerable<AssignmentListDto>>(list);
        }

        public async Task<AssignmentListDto?> GetByIdAsync(int id)
        {
            var entity = await _assignmentRepository.GetByIdAsync(id, a => a.InventoryItem, a => a.Intern, a => a.Employee);
            return _mapper.Map<AssignmentListDto>(entity);
        }

        public async Task<IEnumerable<AssignmentListDto>> GetByEmployeeIdAsync(int employeeId)
        {
            // Repository'de bu metot yoksa GetAllAsync içinde Where ile yapabilirsin
            // Şimdilik Repository'de varmış gibi varsayıyorum
            // Yoksa: _assignmentRepository.GetAllAsync(a => a.EmployeeId == employeeId, ...)

            var list = await _assignmentRepository.GetByEmployeeIdAsync(employeeId);
            return _mapper.Map<IEnumerable<AssignmentListDto>>(list);
        }

        public async Task<IEnumerable<AssignmentListDto>> GetByInternIdAsync(int internId)
        {
            // Repository isimlendirmesi StudentId kalmış olabilir, kontrol et
            var list = await _assignmentRepository.GetByStudentIdAsync(internId);
            return _mapper.Map<IEnumerable<AssignmentListDto>>(list);
        }

        // --- BAKIM OLUŞTURMA (Opsiyonel - MaintenanceService varken burada olması şart değil) ---
        public async Task<int> CreateMaintenanceAsync(MaintenanceCreateDto dto)
        {
            var item = await _inventoryItemRepository.GetByIdAsync(dto.InventoryItemId);
            if (item == null) throw new KeyNotFoundException("Ürün bulunamadı.");

            if (item.Status == ItemStatus.Assigned)
                throw new InvalidOperationException("Ürün şu an zimmetli. Önce iade almalısınız.");

            if (item.Status == ItemStatus.Maintenance)
                throw new InvalidOperationException("Ürün zaten bakımda.");

            var maintenance = _mapper.Map<Maintenance>(dto);
            await _maintenanceRepository.AddAsync(maintenance);

            item.Status = ItemStatus.Maintenance;
            await _inventoryItemRepository.UpdateAsync(item);

            return maintenance.Id;
        }
    }
}