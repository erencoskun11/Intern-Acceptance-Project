using Application.DTOs.Assignment;
using Application.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Stajyer_Projesi.Tests.Systems.Services.AssignmentTests
{
    public class AssignmentTestBase
    {
        protected readonly Mock<IUnitOfWork> _mockUow;
        protected readonly Mock<IMapper> _mockMapper;
        protected readonly Mock<ISignalRService> _mockSignalR;
        protected readonly Mock<IEmailService> _mockEmail;

        // --- 1. Repository Tiplerini Düzeltiyoruz ---
        protected readonly Mock<IAssignmentRepository> _mockAssignmentRepo;
        protected readonly Mock<IEmployeeRepository> _mockEmployeeRepo;
        protected readonly Mock<IInternRepository> _mockInternRepo;

        // BURADAKİ HATALARI DÜZELTTİK: Generic yerine Özel Interface
        protected readonly Mock<IInventoryItemRepository> _mockItemRepo;      // IGeneric... DEĞİL
        protected readonly Mock<IMaintenanceRepository> _mockMaintenanceRepo; // IGeneric... DEĞİL

        protected readonly AssignmentService _sut;

        protected readonly Faker<AssignmentCreateDto> _createFaker;

        public AssignmentTestBase()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockSignalR = new Mock<ISignalRService>();
            _mockEmail = new Mock<IEmailService>();

            // --- 2. Mock Nesnelerini Doğru Tiplerle Oluşturuyoruz ---
            _mockAssignmentRepo = new Mock<IAssignmentRepository>();
            _mockEmployeeRepo = new Mock<IEmployeeRepository>();
            _mockInternRepo = new Mock<IInternRepository>();

            // BURADAKİ HATALARI DÜZELTTİK
            _mockItemRepo = new Mock<IInventoryItemRepository>();
            _mockMaintenanceRepo = new Mock<IMaintenanceRepository>();

            // --- 3. UoW Setup ---
            _mockUow.Setup(x => x.Assignments).Returns(_mockAssignmentRepo.Object);
            _mockUow.Setup(x => x.Employees).Returns(_mockEmployeeRepo.Object);
            _mockUow.Setup(x => x.Interns).Returns(_mockInternRepo.Object);

            // Artık tipler uyuşuyor, hata vermeyecek
            _mockUow.Setup(x => x.InventoryItems).Returns(_mockItemRepo.Object);
            _mockUow.Setup(x => x.Maintenances).Returns(_mockMaintenanceRepo.Object);

            _sut = new AssignmentService(
                _mockUow.Object,
                _mockMapper.Object,
                _mockSignalR.Object,
                _mockEmail.Object
            );

            _createFaker = new Faker<AssignmentCreateDto>()
                .RuleFor(x => x.InventoryItemId, f => f.Random.Int(1, 100))
                .RuleFor(x => x.Notes, f => f.Lorem.Sentence());
        }
    }
}