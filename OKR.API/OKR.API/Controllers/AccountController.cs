﻿using MayNghien.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Infrastructure;
using OKR.Service.Contract;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OKR.API.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IAuthencationService _authencationService;
        public AccountController(IAuthencationService authencationService)
        {
            _authencationService = authencationService;
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
        [AllowAnonymous]
        [Route("test")]
        public IActionResult test()
        {
            return Ok("OK");
        }

        [HttpGet]
        [Route("account-infor")]
        public async Task<IActionResult> GetInforAccount()
        {
            var result =await _authencationService.GetInforAccount();
            return Ok(result);
        }
    }
}
