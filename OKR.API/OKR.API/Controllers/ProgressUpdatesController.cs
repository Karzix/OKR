﻿using MayNghien.Infrastructure.Request.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OKR.Service.Contract;

namespace OKR.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProgressUpdatesController : Controller
    {
        private IProgressUpdatesService _progressUpdatesService;

        public ProgressUpdatesController(IProgressUpdatesService progressUpdatesService)
        {
            _progressUpdatesService = progressUpdatesService;
        }

        [HttpPost]
        public IActionResult Search(SearchRequest request)
        {
            var result =_progressUpdatesService.Search(request);
            return Ok(result);
        }
    }
}
