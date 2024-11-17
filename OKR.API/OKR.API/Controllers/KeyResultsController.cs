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
        public async Task<IActionResult> Update(KeyResultRequest request)
        {
            var result = await _keyResultsService.UpdateWeight(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _keyResultsService.Get(id);
            return Ok(result);
        }
    }
}
