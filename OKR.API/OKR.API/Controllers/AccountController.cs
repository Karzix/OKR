using MayNghien.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IAuthencationService _authencationService;
        private IHttpContextAccessor _contextAccessor;

        public AccountController(IAuthencationService authencationService, IHttpContextAccessor contextAccessor)
        {
            _authencationService = authencationService;
            _contextAccessor = contextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(UserDto login)
        {
            var result = await _authencationService.AuthencationUser(login);
            return Ok(result);
        }
        [HttpPost]
        [Route("Refresh")]
        [AllowAnonymous]
        public IActionResult Refresh(UserDto request)
        {
            var result =  _authencationService.Refresh(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("test")]
        public IActionResult test()
        {
            var UserName = ClaimHelper.GetClainByName(_contextAccessor, "UserName");
            return Ok("OK");
        }
    }
}
