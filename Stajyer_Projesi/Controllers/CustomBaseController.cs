using Microsoft.AspNetCore.Mvc;
using Application.Common; 

namespace API.Controllers
{
    [Route("api/custombase")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(Response<T> response)
        {
            if (response.StatusCode == 204)
                return new ObjectResult(null) { StatusCode = 204 };

            return new ObjectResult(response) { StatusCode = response.StatusCode };
        }
    }
}