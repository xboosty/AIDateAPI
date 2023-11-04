using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace APICore.Services.Utils
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Debug.WriteLine($"{user}: {message}");            
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
