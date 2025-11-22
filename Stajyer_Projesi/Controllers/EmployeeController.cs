using Application.DTOs.Employee;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // GET: api/Employee
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<EmployeeListDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/Employee/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<EmployeeListDto?>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/Employee/by-email?email=...
        [HttpGet("by-email")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<EmployeeListDto?>> GetByEmail([FromQuery] string email)
        {
            var result = await _service.GetByEmailAsync(email);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/Employee/by-department/{departmentId}
        [HttpGet("by-department/{departmentId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<EmployeeListDto>>> GetByDepartment(int departmentId)
        {
            var result = await _service.GetByDepartmentIdAsync(departmentId);
            return Ok(result);
        }

        // POST: api/Employee
        // assumes CreateAsync returns the new entity id (int)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Create([FromBody] EmployeeCreateDto dto)
        {
            var createdId = await _service.CreateAsync(dto);
            return Ok(createdId > 0);
        }

        // PUT: api/Employee/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok(true);
        }

        // DELETE: api/Employee/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(true);
        }
    }
}
