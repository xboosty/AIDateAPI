using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Services.Utils;

namespace APICore.Services
{
    public interface IChatService
    {
        Task<int> CreateChatAsync(int fromId, int toId, int chatId, string msg);
        Task<List<Chat>> GetChatList(int userId);
        Task<bool> SetMessageStatus(int userId, int msgId, int status);
        Task<bool> DeleteMessage(int userId, int msgId);
        Task<PaginatedList<Message>> GetMessageList(int userId, int chatId, int page, int perPage);
    }
}