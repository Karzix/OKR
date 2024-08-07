using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class SidequestsController : Controller
    {
        private ISidequestsService _sidequestsService;

        public SidequestsController(ISidequestsService sidequestsService)
        {
            _sidequestsService = sidequestsService;
        }


        [HttpPut]
        public IActionResult ChangeStatus(SidequestsDto request)
        {
            var result =_sidequestsService.ChangeStatus(request);
            return Ok(result);
        }
    }
}
