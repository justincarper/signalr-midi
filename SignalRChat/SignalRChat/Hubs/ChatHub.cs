using Microsoft.AspNetCore.SignalR;
using SignalRChat.Models;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(ChatMsg msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}