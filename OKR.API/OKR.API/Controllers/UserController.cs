using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserService _userService;
        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }


        [HttpPost]
        public async Task<IActionResult> Create(UserDto request)
        {
            var result =await _userService.Create(request);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> Update(UserDto request)
        {
            var result =await _userService.Update(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("lock")]
        public async Task<IActionResult> Lock(string Id)
        {
            var result = await _userService.LockAsync(Id);
            return Ok(result);
        }
        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(SearchRequest request)
        {
            var result =await _userService.Search(request);
            return Ok(result);
        }
    }
}
