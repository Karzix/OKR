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
    [Authorize(Roles = "Teamleader, BranchManagement")]
    public class DepartmentProgressApprovalController : Controller
    {
        private IDepartmentProgressApprovalService _service;
        public DepartmentProgressApprovalController(IDepartmentProgressApprovalService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("search")]
        public IActionResult search([FromQuery]SearchRequest request)
        {
            var result = _service.Search(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("confirm")]
        public IActionResult Confirm(DepartmentProgressApprovalRequest dto)
        {
            var result = _service.Confirm(dto);
            return Ok(result);
        }
    }
} 
