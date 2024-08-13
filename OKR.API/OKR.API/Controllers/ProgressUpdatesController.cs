using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProgressUpdatesController : Controller
    {
        private IProgressUpdatesService _progressUpdatesService;

        public ProgressUpdatesController(IProgressUpdatesService progressUpdatesService)
        {
            _progressUpdatesService = progressUpdatesService;
        }

        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public IActionResult Search(SearchRequest request)
        {
            var result =_progressUpdatesService.Search(request);
            return Ok(result);
        }
        [HttpPost]
        [Route("data-chart")]
        [AllowAnonymous]
        public IActionResult GetDataChart(SearchRequest? request)
        {
            request = new SearchRequest();
            request.Filters = new List<Filter>();
            request.Filters.Add(new Filter
            {
                FieldName = "keyresultsId",
                Value = "8A3C69FD-F6EA-4835-98B6-992626BE5F54",
            });
            var result = _progressUpdatesService.GetDataChart(request);
            return Ok(result);
        }
    }
}
