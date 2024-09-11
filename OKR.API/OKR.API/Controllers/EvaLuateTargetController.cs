using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;
using OKR.Service.Implementation;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EvaLuateTargetController : Controller
    {
        private IEvaluateTargetService _evaluateTargetService;
        public EvaLuateTargetController(IEvaluateTargetService evaluateTargetService)
        {
            _evaluateTargetService = evaluateTargetService;
        }
        [HttpPost]
        public IActionResult Create(EvaluateTargetDto request)
        {
            var result = _evaluateTargetService.Create(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var result = _evaluateTargetService.Delete(Id);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult Edit(EvaluateTargetDto request)
        {
            var reuslt = _evaluateTargetService.Edit(request);
            return Ok(reuslt);
        }
        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _evaluateTargetService.Search(request);
            return Ok(result);
        }
    }
}
