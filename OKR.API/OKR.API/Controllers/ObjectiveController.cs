using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ObjectiveController : Controller
    {
        private IObjectiveService _objectiveService;

        public ObjectiveController(IObjectiveService objectiveService)
        {
            _objectiveService = objectiveService;
        }

        [HttpPost]
        public IActionResult Create(ObjectiveDto request)
        {
            var result = _objectiveService.Create(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var result = _objectiveService.Get(Id);
            return Ok(result);
        }

    }
}
