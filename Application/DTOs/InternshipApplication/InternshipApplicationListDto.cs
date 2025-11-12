using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.InternshipApplication
{
    public class InternshipApplicationListDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public string? EmployeeName { get; set; } // Bu alan eklendi
        public string Role { get; set; } = null!;
        public decimal TaskSuccessPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsVolunteer { get; set; }
    }
}
