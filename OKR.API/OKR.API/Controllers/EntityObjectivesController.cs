using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntityObjectivesController : Controller
    {

        private IEntityObjectivesService _entityObjectivesService;
        public EntityObjectivesController(IEntityObjectivesService entityObjectivesService)
        {
            _entityObjectivesService = entityObjectivesService;
        }

        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result  = _entityObjectivesService.Search(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _entityObjectivesService.Get(id);
            return Ok(result);
        }
        [HttpPut]
        [Route("status-change")]
        public IActionResult StatusChange(EntityObjectivesDto request)
        {
            var result = _entityObjectivesService.StatusChange(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _entityObjectivesService.DeleteEntityObjectives(id);
            return Ok(result);
        }
    }
}
