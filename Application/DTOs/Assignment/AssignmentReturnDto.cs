using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Assignment
{
    public class AssignmentReturnDto
    {
        public int AssignmentId { get; set; }
        public DateTime? ActualReturnAt { get; set; }
    }
}
