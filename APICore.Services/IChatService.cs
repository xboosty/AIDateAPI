using APICore.Common.DTO.Request;
using APICore.Data.Entities;

namespace APICore.Services
{
    public interface IChatService
    {
        Task<bool> CreateChatAsync(int fromId, int toId, string msg);
        Task<List<ChatUsers>> GetChatList(int userId);
        Task<bool> SetMessageStatus(int userId, int msgId, int status);
        Task<bool> DeleteMessage(int userId, int msgId);
    }
}