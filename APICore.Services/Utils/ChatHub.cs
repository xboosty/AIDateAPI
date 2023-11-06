using APICore.Common.DTO.Response;
using APICore.Data.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace APICore.Services.Utils
{
    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _uow;
public ChatHub(IUnitOfWork uow)
        {
            _uow=uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task SendMessage(string user, string message)
        {
            Debug.WriteLine($"{user}: {message}");            
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task UpdateChat(int toId, string msg)
        {
                //Debug.WriteLine($"{user}: {message}");
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var connectionId = Context.ConnectionId;
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);

            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                await _uow.HubConnectionRepository.AddAsync(new Data.Entities.UserHubConnection { UserId = userId, ConnectionId = connectionId });
                await _uow.CommitAsync();
            }

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;
            var connectionId = Context.ConnectionId;
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);

            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                var connection = await _uow.HubConnectionRepository.FirstOrDefaultAsync(c => c.ConnectionId.Equals(connectionId) && c.UserId == userId);
                if (connection != null)
                {
                    _uow.HubConnectionRepository.Delete(connection);
                    await _uow.CommitAsync();
                        }
            }

        }
    }
}
