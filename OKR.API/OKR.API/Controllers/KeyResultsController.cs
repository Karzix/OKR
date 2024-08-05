﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.DTO;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeyResultsController : Controller
    {
        private IKeyResultsService _keyResultsService;

        public KeyResultsController(IKeyResultsService keyResultsService)
        {
            _keyResultsService = keyResultsService;
        }

        [HttpPut]
        public IActionResult Update(KeyResultDto request)
        {
            var result =_keyResultsService.Update(request);
            return Ok(result);
        }

    }
}