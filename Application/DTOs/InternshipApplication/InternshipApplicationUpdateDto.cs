using System;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.InternshipApplication
{
    public class InternshipApplicationUpdateDto
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int DepartmentId { get; set; }

        public InternshipRole Role { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsMandatory { get; set; } = false;
        public bool IsVolunteer { get; set; } = false;

        public int? MentorId { get; set; }

        public decimal TaskSuccessPercent { get; set; }
    }
}
