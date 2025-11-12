using Application.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IStudentService : IGenericService<StudentListDto,StudentCreateDto,StudentUpdateDto>    
    {
    }
}
