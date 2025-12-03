using Application.Common;
using Application.DTOs.Intern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IInternService : IGenericService<InternListDto,InternCreateDto,InternUpdateDto>    
    {
        Task<Response<IEnumerable<InternListDto>>> GetAllWithDetailsAsync();
    }
}
