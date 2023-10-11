using APICore.Data.Entities;

namespace APICore.Services
{
    public interface IBlockService
    {
        Task<bool> BlockUserAsync(int blockerUserId, int blockedUserId);
        Task<bool> UnblockUserAsync(int blockerUserId, int blockedUserId);
        Task<List<User>> GetBlockedUserList(int userId);
    }
}