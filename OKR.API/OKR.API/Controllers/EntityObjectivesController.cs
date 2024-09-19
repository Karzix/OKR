using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntityObjectivesController : Controller
    {

        private IEntityObjectivesService _userObjectivesService;
        public EntityObjectivesController(IEntityObjectivesService userObjectivesService)
        {
            _userObjectivesService = userObjectivesService;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result  = _userObjectivesService.Search(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _userObjectivesService.Get(id);
            return Ok(result);
        }
    }
}
