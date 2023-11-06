using APICore.Common.DTO.Request;
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

        public async Task<int> CreateChatAsync(int fromId, int toId,int chatId, string msg)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == fromId) ?? throw new UserNotFoundException(_localizer);
            var userTo = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == toId)  ?? throw new UserNotFoundException(_localizer);
            Chat chat = new Chat();
            Message message = new Message {SenderId = fromId, Content = msg};
            if (chatId == 0)
            {
                chat = new Chat();
                chat.Messages.Add(message);
                chat.Participants.Add(new ChatParticipation { UserId = fromId});
                chat.Participants.Add(new ChatParticipation { UserId = toId});
                await _uow.ChatRepository.AddAsync(chat);
                await _uow.CommitAsync();
                return chat.Id;
            }

            chat = await _uow.ChatRepository.GetAll()
                            .Include(c => c.Messages)
                            .FirstOrDefaultAsync(c => c.Id == chatId);
            chat.Messages.Add(message);
            await _uow.ChatRepository.UpdateAsync(chat, chatId);
            await _uow.CommitAsync();

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

            return await  PaginatedList<Message>.CreateAsync(messagesList, page, perPage);
        }

        public async Task<bool> SetMessageStatus(int userId, int msgId, int status)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var message = await _uow.MessageRepository.FirstOrDefaultAsync(m => m.Id == msgId && m.SenderId == userId) ?? throw new MessageNotFoundException(_localizer);
message.Status =(MessageStatusEnum) status;
            await _uow.MessageRepository.UpdateAsync(message, msgId);
            await _uow.CommitAsync();

            return true; ;
        }

    }
}