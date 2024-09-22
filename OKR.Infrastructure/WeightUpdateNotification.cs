using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace OKR.Infrastructure
{
    public class WeightUpdateNotification : Hub
    {
        private IConfiguration _configuration;
        public WeightUpdateNotification(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync(SignalRMessage.WeightUpdate, message);
        }
    }
}
