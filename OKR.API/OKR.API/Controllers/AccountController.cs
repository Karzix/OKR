using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("OKR-api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAuthencationService _authencationService;

        public AccountController(IAuthencationService authencationService)
        {
            _authencationService = authencationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserDto login)
        {
            var result = await _authencationService.AuthencationUser(login);
            return Ok(result);
        }
        [HttpPost]
        [Route("Refresh")]
        public IActionResult Refresh(UserDto request)
        {
            var result =  _authencationService.Refresh(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("test")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public IActionResult test()
        {
            return Ok("OK");
        }
    }
}
