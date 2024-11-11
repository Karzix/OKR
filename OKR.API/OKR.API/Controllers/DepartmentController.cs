using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;
using OKR.Service.Implementation;
using System.Diagnostics.Metrics;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, superadmin")]
    public class DepartmentController : Controller
    {
        private IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result =_departmentService.GetAll();
            return Ok(result);
        }
        [HttpGet]
        [Route("{Id}")]
        public IActionResult Get(Guid Id)
        {
            var reuslt = _departmentService.Get(Id);
            return Ok(reuslt);
        }
        [HttpPost]
        public IActionResult Create(DepartmentDto request)
        {
            var result = _departmentService.Create(request);
            return Ok(result);
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult Delete(Guid Id)
        {
            var result = _departmentService.Delete(Id);
            return Ok(result);
        }
        [HttpPut]
        public IActionResult Edit(DepartmentDto request)
        {
            var reuslt = _departmentService.Update(request);
            return Ok(reuslt);
        }
        [HttpPost]
        [Route("search")]
        public IActionResult Search(SearchRequest request)
        {
            var result = _departmentService.Search(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("parent-department-by-level/{level}")]
        public IActionResult ParentDepartmentByLevel(int level)
        {
            var result = _departmentService.GetParentDepartmentByLevel(level);
            return Ok(result);
        }
        [HttpGet]
        [Route("department-level")]
        public IActionResult DepartmentLevelNumber()
        {
            var result = _departmentService.DepartmentLevelNumber();
            return Ok(result);
        }
        [HttpGet]
        [Route("list-by-keyword/{keyword}")]
        [AllowAnonymous]
        public IActionResult GetListByKeyword(string keyword)
        {
            var result = _departmentService.GetDepartByKeyword(keyword);
            return Ok(result);
        }
    }
}
