using Application.Common;
using Application.DTOs.Assignment;
using Application.DTOs.Maintenance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IAssignmentService : IGenericService<AssignmentListDto, AssignmentCreateDto, AssignmentUpdateDto>
    {

        Task<Response<NoContent>> ReturnAsync(AssignmentReturnDto dto);
        Task<Response<int>> CreateMaintenanceAsync(MaintenanceCreateDto dto);

        Task<Response<IEnumerable<AssignmentListDto>>> GetByEmployeeIdAsync(int employeeId);
        Task<Response<IEnumerable<AssignmentListDto>>> GetByInternIdAsync(int internId);
    }
}