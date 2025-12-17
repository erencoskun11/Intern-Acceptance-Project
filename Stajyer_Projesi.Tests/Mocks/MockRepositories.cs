using Application.Interfaces; // IUnitOfWork ve IEmployeeRepository için
using Domain.Entities;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stajyer_Projesi.Tests.Mocks
{
    public static class MockRepositories
    {
        public static Mock<IEmployeeRepository> GetEmployeeRepository()
        {
            var mockRepo = new Mock<IEmployeeRepository>();

            // HATA 1 ÇÖZÜMÜ: GetAllAsync satırını sildik. 
            // Şu anki Create testlerinde buna ihtiyacımız yok.
            // İleride lazım olursa o testin içinde Setup ederiz.

            return mockRepo;
        }
        public static Mock<IUnitOfWork> GetUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();

            // HATA 2 ÇÖZÜMÜ: Task.CompletedTask yerine ReturnsAsync(1) kullandık.
            // Çünkü CommitAsync metodu geriye int (Task<int>) dönüyor.
            mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            return mockUow;
        }

    }
}