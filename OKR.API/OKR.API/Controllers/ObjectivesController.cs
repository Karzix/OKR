using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;
using OKR.Service.Implementation;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ObjectivesController : Controller
    {
        private IObjectivesService _objectiveService;

        public ObjectivesController(IObjectivesService objectiveService)
        {
            _objectiveService = objectiveService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ObjectiveDto request)
        {
            var result = await _objectiveService.Create(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var result = _objectiveService.Get(Id);
            return Ok(result);
        }

        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public IActionResult Search(SearchRequest request)
        {
            var result = _objectiveService.Search(request);
            return Ok(result);
        }
        [HttpPost]
        [Route("overal-progress")]
        [AllowAnonymous]
        public IActionResult OverallProgress(SearchRequest request)
        {
            var result = _objectiveService.CaculateOveralProgress(request);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult Edit(ObjectiveDto request)
        {
            var result = _objectiveService.Edit(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var result = _objectiveService.Delete(id);
            return Ok(result);
        }
        [HttpGet]
        [Route("periods")]
        public IActionResult GetPeriod()
        {
            var result= _objectiveService.GetPeriods();
            return Ok(result);
        }
    }
}
