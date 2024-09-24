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
    public class KeyResultsController : Controller
    {
        private IKeyResultsService _keyResultsService;

        public KeyResultsController(IKeyResultsService keyResultsService)
        {
            _keyResultsService = keyResultsService;
        }

        [HttpPut]
        public async Task<IActionResult> Update(KeyResultDto request)
        {
            var result = await _keyResultsService.Update(request);
            return Ok(result);
        }

    }
}
