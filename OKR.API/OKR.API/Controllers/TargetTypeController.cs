using MayNghien.Infrastructure.Request.Base;
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
    public class TargetTypeController : Controller
    {
        private ITargetTypeService _targetTypeService;
        public TargetTypeController(ITargetTypeService targetTypeService)
        {
            _targetTypeService = targetTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var result = _targetTypeService.GetAll();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var reuslt = _targetTypeService.Get(Id);
            return Ok(reuslt);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(TargetTypeDto request)
        {
            var result = _targetTypeService.Create(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var result = _targetTypeService.Delete(Id);
            return Ok(result);
        }
        [HttpPut]
        //[Route("{Id}")]
        public IActionResult Edit(TargetTypeDto request)
        {
            var reuslt = _targetTypeService.Edit(request);
            return Ok(reuslt);
        }
        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _targetTypeService.Search(request);
            return Ok(result);
        }
    }
}
