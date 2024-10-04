using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OKR.Service.Implementation;
using Quartz;
using System.Threading.Tasks;

namespace OKR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : Controller
    {
        private readonly SendReminderEmailJob _sendReminderEmailJob;

        public TestController(SendReminderEmailJob sendReminderEmailJob)
        {
            _sendReminderEmailJob = sendReminderEmailJob;
        }

        [HttpGet("send-test-email")]
        public async Task<IActionResult> SendTestEmail()
        {
            // Tạo một context giả để gọi hàm Execute của job
            await _sendReminderEmailJob.Execute(null);

            return Ok("Test email sent successfully!");
        }
    }
}
