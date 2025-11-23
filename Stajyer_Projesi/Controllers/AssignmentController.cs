using Application.DTOs.Assignment;
using Application.DTOs.Maintenance;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _service;

        public AssignmentController(IAssignmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<AssignmentListDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<AssignmentListDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create(AssignmentCreateDto dto)
        {
            try
            {
                var id = await _service.AssignAsync(dto);
                return Ok(id);
            }
            catch (Application.Exceptions.ItemNotAvailableException ex)
            {
                // Item başka kullanıcıda, 400 BadRequest dön
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        // Return (iade)
        [HttpPost("return")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Return(AssignmentReturnDto dto)
        {
            await _service.ReturnAsync(dto);
            return Ok();
        }

        // Create maintenance
        [HttpPost("maintenance")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> CreateMaintenance(MaintenanceCreateDto dto)
        {
            var id = await _service.CreateMaintenanceAsync(dto);
            return Ok(id);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AssignmentUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("by-employee/{employeeId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            var list = await _service.GetByEmployeeIdAsync(employeeId);
            return Ok(list);
        }

        [HttpGet("by-student/{internId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ByIntern(int studentId)
        {
            var list = await _service.GetByInternIdAsync(studentId);
            return Ok(list);
        }
    }
}

