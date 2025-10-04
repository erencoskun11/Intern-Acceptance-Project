using Application.DTOs.Department;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentListDto>>> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentListDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Create(DepartmentCreateDto dto) => Ok(await _service.CreateAsync(dto) > 0);

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, DepartmentUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return true;
        }
    }
}
