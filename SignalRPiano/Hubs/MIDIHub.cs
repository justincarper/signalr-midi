using Microsoft.AspNetCore.SignalR;
using SharedModels;
using System.Threading.Tasks;

namespace SignalRPiano.Hubs
{
    public class MIDIHub : Hub
    {
        public async Task SendMessage(MIDIMessage msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", msg);
        }
    }
}