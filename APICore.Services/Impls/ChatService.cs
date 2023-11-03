using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using APICore.Services.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IChatService> _localizer;

        public ChatService(IUnitOfWork uow, IStringLocalizer<IChatService> localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<bool> CreateChatAsync(int fromId, int toId, string msg)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == fromId) ?? throw new UserNotFoundException(_localizer);
            var userTo = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == toId)  ?? throw new UserNotFoundException(_localizer);
            var message = new ChatUsers
            {
                ToId = toId,
                FromId = fromId,
                Message = msg,
                messageStatus = MessageStatusEnum.SEND,
                Created = DateTime.Now
            };
            await _uow.ChatUsersRepository.AddAsync(message);
            await _uow.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteMessage(int userId, int msgId)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var message = await _uow.ChatUsersRepository.FirstOrDefaultAsync(m => m.Id == msgId) ?? throw new MessageNotFoundException(_localizer);
            _uow.ChatUsersRepository.Delete(message);
            await _uow.CommitAsync();
            return true;
        }

        public async Task<List<ChatUsers>> GetChatList(int userId)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var msgList = await _uow.ChatUsersRepository.GetAll()
                            .Where(m => m.FromId == userId)
                            .Include(m => m.From)
                            .Include(m => m.To)
                            .ToListAsync();

            return msgList; ;
        }

        public async Task<bool> SetMessageStatus(int userId, int msgId, int status)
        {
            var userFrom = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var message = await _uow.ChatUsersRepository.FirstOrDefaultAsync(m => m.Id == msgId) ?? throw new MessageNotFoundException(_localizer);
message.messageStatus =(MessageStatusEnum) status;
            await _uow.ChatUsersRepository.UpdateAsync(message, msgId);
            await _uow.CommitAsync();

            return true; ;
        }
    }
}