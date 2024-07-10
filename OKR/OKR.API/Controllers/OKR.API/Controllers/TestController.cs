using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OKR.API.Controllers
{
    [Route("OKR-api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("OK");
        }
    }
}
