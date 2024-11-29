﻿using MayNghien.Infrastructure.Request.Base;
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
    public class UserController : Controller
    {
        private IUserService _userService;
        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }


        [HttpPost]
        [Authorize(Roles ="superadmin, Admin")]
        public async Task<IActionResult> Create(UserRequest request)
        {
            var result =await _userService.Create(request);
            return Ok(result);
        }
        [HttpPut]
        [Authorize(Roles = "superadmin, Admin")]
        public async Task<IActionResult> Update(UserRequest request)
        {
            var result =await _userService.Update(request);
            return Ok(result);
        }
        [HttpPut]
        [Route("lock")]
        [Authorize(Roles = "superadmin, Admin")]
        public async Task<IActionResult> Lock(string userId, bool isLock)
        {
            var result = await _userService.LockAsync(userId, isLock);
            return Ok(result);
        }
        [HttpGet]
        [Route("search")]
        [Authorize(Roles = "superadmin, Admin")]
        public async Task<IActionResult> Search([FromQuery]SearchRequest request)
        {
            var result =await _userService.Search(request);
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = "superadmin, Admin")]
        public IActionResult GetAll()
        {
            var result =_userService.GetAll();
            return Ok(result);
        }
        [HttpGet]
        [Route("{userName}")]
        public async Task<IActionResult> Get(string userName)
        {
            var result = await _userService.Get(userName);
            return Ok(result);
        }
        [HttpGet]
        [Route("list-by-keyword/{username}")]
        public IActionResult GetListByKeyword(string username)
         {
            var result = _userService.GetListByKeyworld(username);
            return Ok(result);
        }
        [HttpPut]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(UserRequest request)
        {
            var result = await _userService.ChangePassword(request);
            return Ok(result);
        }
    }
}
