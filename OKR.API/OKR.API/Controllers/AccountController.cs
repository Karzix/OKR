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
    }
}
