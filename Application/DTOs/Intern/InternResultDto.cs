using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Intern
{
    public class InternResultDto
    {
        // SQL fonksiyonundaki "NewId" kolonunu karşılar
        public int NewId { get; set; }

        // SQL fonksiyonundaki "ErrorMessage" kolonunu karşılar
        public string? ErrorMessage { get; set; }
    }
}
