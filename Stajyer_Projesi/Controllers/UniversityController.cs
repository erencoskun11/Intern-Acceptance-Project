using Application.Common;
using Application.DTOs.University;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace API.Controllers
{
    [Route("api/universities")]
    [ApiController]
    public class UniversityController : CustomBaseController
    {
        private readonly IUniversityService _service;

        public UniversityController(IUniversityService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAll()
        {
            // DÜZELTME: Response.Success KULLANMA. Servis zaten Response dönüyor.
            return CreateActionResult(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetById(int id)
        {
            return CreateActionResult(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UniversityCreateDto dto)
        {
            return CreateActionResult(await _service.CreateAsync(dto));
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkCreate([FromBody] IEnumerable<UniversityCreateDto> dtos)
        {
            return CreateActionResult(await _service.BulkCreateAsync(dtos));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UniversityUpdateDto dto)
        {
            return CreateActionResult(await _service.UpdateAsync(id, dto));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return CreateActionResult(await _service.DeleteAsync(id));
        }


        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {

            if (file == null || file.Length == 0)
                return CreateActionResult(Response<NoContent>.Fail("Dosya seçilmedi.", 400, true));

            var dtos = new List<UniversityCreateDto>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        if (worksheet.Dimension == null)
                            return CreateActionResult(Response<NoContent>.Fail("Excel dosyası boş.", 400, true));

                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var uniName = worksheet.Cells[row, 1].Value?.ToString()?.Trim();

                            if (!string.IsNullOrEmpty(uniName))
                            {
                                dtos.Add(new UniversityCreateDto { Name = uniName });
                            }
                        }
                    }
                }

                if (dtos.Any())
                {
                    return CreateActionResult(await _service.BulkCreateAsync(dtos));
                }

                return CreateActionResult(Response<NoContent>.Fail("Excel içinde okunacak geçerli veri bulunamadı.", 400, true));
            }
            catch (Exception ex)
            {
                return CreateActionResult(Response<NoContent>.Fail($"Excel hatası: {ex.Message}", 500, true));
            }
        }













    }
}