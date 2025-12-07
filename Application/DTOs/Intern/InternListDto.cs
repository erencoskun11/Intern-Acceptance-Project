namespace Application.DTOs.Intern
{
    public class InternListDto
    {
        public int Id { get; set; }

        // --- BU İKİ SATIRI EKLEMEN GEREKİYOR ---
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        // ---------------------------------------

        public string FullName { get; set; } = null!; // Bu da kalsın, tabloda birleşik gösteriyoruz
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? StudentNumber { get; set; }
        public int ClassYear { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public int Degree { get; set; }
        public int WorkDaysPerWeek { get; set; }

        public string UniversityName { get; set; } = null!;

        // Edit işlemi için ID'ler de lazım olabilir
        public int UniversityId { get; set; }
    }
}