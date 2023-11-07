using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using APICore.Services.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IChatService> _localizer;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(IUnitOfWork uow, IStringLocalizer<IChatService> localizer, IHubContext<ChatHub> hubContext)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<int> CreateChatAsync(int fromId, int toId, int chatId, string msg)
        {
            var userFrom = await _uow.UserRepository.GetAll()
                .Include(u => u.ActiveConnections)
                .FirstOrDefaultAsync(u => u.Id == fromId) ?? throw new UserNotFoundException(_localizer);
            var userTo = await _uow.UserRepository.GetAll()
                .Include(u => u.ActiveConnections)
                .FirstOrDefaultAsync(u => u.Id == toId)  ?? throw new UserNotFoundException(_localizer);

            Chat chat = new Chat();
            Message message = new Message { Sender = userFrom, Content = msg, SentDate = DateTime.Now, Status = MessageStatusEnum.SEND};
            if (chatId == 0)
            {
                chat = new Chat();
                chat.Messages.Add(message);
                chat.Participants.Add(new ChatParticipation { UserId = fromId });
                chat.Participants.Add(new ChatParticipation { UserId = toId });
                await _uow.ChatRepository.AddAsync(chat);
                await _uow.CommitAsync();
                var connectionIds = userFrom.ActiveConnections.Select(c => c.ConnectionId).Union(userTo.ActiveConnections.Select(c => c.ConnectionId)).ToList();
                await UpdateChat(connectionIds, message);
                return chat.Id;
            }

            chat = await _uow.ChatRepository.GetAll()
                            .Include(c => c.Messages)
                            .Include(c => c.Participants)
                            .ThenInclude(c => c.User)
                            .ThenInclude(u => u.ActiveConnections)
                            .FirstOrDefaultAsync(c => c.Id == chatId);
            chat.Messages.Add(message);
            await _uow.ChatRepository.UpdateAsync(chat, chatId);
            await _uow.CommitAsync();
            var idConnections = chat.Participants.SelectMany(p => p.User.ActiveConnections.Select(c => c.ConnectionId)).ToList();
            await UpdateChat(idConnections, message);

            return chat.Id;
        }

        public async Task<bool> DeleteMessage(int userId, int msgId)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var message = await _uow.MessageRepository.FirstOrDefaultAsync(m => m.Id == msgId && m.SenderId == userId) ?? throw new MessageNotFoundException(_localizer);
            _uow.MessageRepository.Delete(message);
            await _uow.CommitAsync();
            return true;
        }

        public async Task<List<Chat>> GetChatList(int userId)
        {
            var user = await _uow.UserRepository.GetAll()
                .Include(u => u.ParticipatedChats)
                .ThenInclude(p => p.Chat)
                .ThenInclude(c => c.Participants)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);

            user.ParticipatedChats.ToList().ForEach(chat =>
            {
                chat.Chat.Participants = chat.Chat.Participants.Where(participant => participant.UserId != userId).ToList();
            });

            return user.ParticipatedChats.Select(c => c.Chat).ToList();
        }

        public async Task<PaginatedList<Message>> GetMessageList(int userId, int chatId, int page, int perPage)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var messagesList = _uow.MessageRepository.GetAll()
                            .Where(message => message.ChatId == chatId);

            return await PaginatedList<Message>.CreateAsync(messagesList, page, perPage);
        }

        public async Task<bool> SetMessageStatus(int userId, int msgId, int status)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var message = await _uow.MessageRepository.FirstOrDefaultAsync(m => m.Id == msgId && m.SenderId == userId) ?? throw new MessageNotFoundException(_localizer);
            message.Status =(MessageStatusEnum)status;
            await _uow.MessageRepository.UpdateAsync(message, msgId);
            await _uow.CommitAsync();

            return true; ;
        }

        public async Task<PaginatedList<User>> GetUserList(int userId,string name, int page, int perPage)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var blockedIds = await _uow.BlockedUsersRepository.GetAll()
                .Where(b => b.BlockerUserId == userId)
                .Select(b => b.BlockedUserId).ToListAsync();

            var users = _uow.UserRepository.GetAll()
                .Where(u => !blockedIds.Contains(u.Id));
            if (!string.IsNullOrEmpty(name))
                users = users.Where(u => u.FullName.Contains(name)).OrderBy(u => u.FullName);

            return await PaginatedList<User>.CreateAsync(users, page, perPage);
        }

        private async Task UpdateChat(List<string> connections, Message msg)
        {
            var response = new
            {
                ChatId = msg.ChatId,
                Message = msg.Content,
                sender = msg.Sender.FullName
            };

            await _hubContext.Clients.Clients(connections).SendAsync("UpdateChat", response);
        }
    }
}