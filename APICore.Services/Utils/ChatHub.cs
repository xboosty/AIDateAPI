using APICore.Common.DTO.Response;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        public async Task UpdateMessage(int messageId, MessageStatusEnum status)
        {
            var user = Context.User;
            var connectionId = Context.ConnectionId;
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);
            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);

                var _user = await _uow.UserRepository.GetAll()
                    .Include(u => u.ActiveConnections)
                    .Include(u => u.ParticipatedChats)
                    .ThenInclude(p => p.User)
                    .ThenInclude(u => u.ActiveConnections)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                var msg = await _uow.MessageRepository.FirstOrDefaultAsync(m => m.Id == messageId);
                msg.Status = status;
                await _uow.MessageRepository.UpdateAsync(msg, messageId);
                await _uow.CommitAsync();
                var connectionIds = _user.ParticipatedChats.SelectMany(p => p.User.ActiveConnections.Select(c => c.ConnectionId)).ToList();

                await Clients.Clients(connectionIds).SendAsync("UpdateMessageStatus", messageId, msg.Status.ToString());

            }
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var connectionId = Context.ConnectionId;
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData);

            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);

                var _user = await _uow.UserRepository.GetAll()
                    .Include(u => u.ActiveConnections)
                    .Include(u => u.ParticipatedChats)
                    .ThenInclude(p => p.User)
                    .ThenInclude(u => u.ActiveConnections)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (_user.ActiveConnections.Count() == 0)
                {
                    var connectionIds = _user.ParticipatedChats.SelectMany(p => p.User.ActiveConnections.Select(c => c.ConnectionId)).ToList();
                    _user.ChatStatus = Data.Entities.Enums.ChatStatusEnum.ONLINE;
                    await Clients.Clients(connectionIds).SendAsync("UpdateStatus", userId, _user.ChatStatus.ToString());
                    _user.ActiveConnections.Add(new Data.Entities.UserHubConnection { ConnectionId = connectionId });
                    await _uow.UserRepository.UpdateAsync(_user, userId);
                    await _uow.CommitAsync();
                }
                else
                {
                    _user.ActiveConnections.Add(new Data.Entities.UserHubConnection { ConnectionId = connectionId });
                    await _uow.UserRepository.UpdateAsync(_user, userId);
                    await _uow.CommitAsync();
                }

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
                var _user = await _uow.UserRepository.GetAll()
                    .Include(u => u.ActiveConnections)
                    .Include(u => u.ParticipatedChats)
                    .ThenInclude(p => p.User)
                    .ThenInclude(u => u.ActiveConnections)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (connection != null)
                {
                    _user.ActiveConnections.Remove(connection); await _uow.CommitAsync();
                    if (_user.ActiveConnections.Count() == 0)
                    {
                        _user.ChatStatus = Data.Entities.Enums.ChatStatusEnum.OFFLINE;
                        var connectionIds = _user.ParticipatedChats.SelectMany(p => p.User.ActiveConnections.Select(c => c.ConnectionId)).ToList();
                        await Clients.Clients(connectionIds).SendAsync("UpdateStatus", userId, _user.ChatStatus.ToString());
                    }
                    await _uow.UserRepository.UpdateAsync(_user, userId);
                    await _uow.CommitAsync();

                }
            }

        }
    }
}
