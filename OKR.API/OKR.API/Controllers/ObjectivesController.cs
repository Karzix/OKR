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
        public async Task<IActionResult> Create(ObjectivesRequest request)
        {
            var result = await _objectiveService.Create(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            var result = _objectiveService.Get(id);
            return Ok(result);
        }

        [HttpGet]
        [Route("search")]
        //[AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] SearchRequest request)
        {
            var result = await _objectiveService.Search(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("overal-progress")]
        [AllowAnonymous]
        public async Task<IActionResult> OverallProgress([FromQuery] SearchRequest request)
        {
            var result = await _objectiveService.CaculateOveralProgress(request);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult Edit(ObjectivesRequest request)
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
        [HttpGet]
        [Route("statusStatistics")]
        public async Task<IActionResult> GetStatusStatistics([FromQuery] SearchRequest request)
        {
            var result = await _objectiveService.StatusStatistics(request);
            return Ok(result);
        }
    }
}
