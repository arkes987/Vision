using Microsoft.AspNetCore.SignalR;
using VisionAPI.Models;

namespace VisionAPI.Hubs
{
    public class ImageHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            if(Clients != null)
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
            }
        }
    }
}
