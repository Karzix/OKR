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
        public async Task ReceiveMessageWeightUpdate(string connectionID,string message)
        {
            await Clients.All.SendAsync(SignalRMessage.WeightUpdate + connectionID, message);
        }
    }
}
