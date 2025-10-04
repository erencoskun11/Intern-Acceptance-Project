using Application.DTOs.University;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityService _service;

        public UniversityController(IUniversityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UniversityListDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UniversityListDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost("bulk")]
        public async Task<ActionResult<bool>> BulkCreate([FromBody] IEnumerable<UniversityCreateDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return BadRequest("At least one university is required.");

            await _service.BulkCreateAsync(dtos);
            return true;
        }


        [HttpPost]
        public async Task<ActionResult<bool>> Create(UniversityCreateDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return id > 0;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(int id, UniversityUpdateDto dto)
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
