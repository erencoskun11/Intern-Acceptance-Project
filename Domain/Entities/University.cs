using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class University : BaseEntity
    {
        public string Name { get; set; } = null!;

        public string? City { get; set; }

        public ICollection<Intern>? Students { get; set; }
    }
}
