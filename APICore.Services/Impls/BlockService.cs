using APICore.Data.Entities;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace APICore.Services.Impls
{

    public class BlockService : IBlockService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IBlockService> _localizer;

        public BlockService(IUnitOfWork uow, IStringLocalizer<IBlockService> localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<bool> BlockUserAsync(int blockerUserId, int blockedUserId)
        {
            var blockerUser = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == blockerUserId) ?? throw new UserNotFoundException(_localizer);
            var blockedUser = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == blockedUserId) ?? throw new UserNotFoundException(_localizer);
            var blockedList = (!string.IsNullOrEmpty(blockerUser.BlockedUsers)) ? JsonConvert.DeserializeObject<List<BlockedUsers>>(blockerUser.BlockedUsers) : new List<BlockedUsers>();

            if (blockedList.Any(u => u.BlockedUserId == blockedUserId))
            {
                return true;
            }

            var newBlock = new BlockedUsers
            {
                BlockedUserId = blockedUserId,
                BlockDateTime = DateTime.UtcNow
            };

            blockedList.Add(newBlock);
            blockerUser.BlockedUsers = JsonConvert.SerializeObject(blockedList);
            await _uow.UserRepository.UpdateAsync(blockerUser, blockerUserId);
            await _uow.CommitAsync();

            return true;
        }

        public async Task<bool> UnblockUserAsync(int blockerUserId, int blockedUserId)
        {
            var blockerUser = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == blockerUserId) ?? throw new UserNotFoundException(_localizer);
            var blockedList = (!string.IsNullOrEmpty(blockerUser.BlockedUsers))? JsonConvert.DeserializeObject<List<BlockedUsers>>(blockerUser.BlockedUsers): new List<BlockedUsers>();

            if (blockedList.Any(b => b.BlockedUserId == blockedUserId))
            {
                var blockedUser = blockedList.FirstOrDefault(b => b.BlockedUserId == blockedUserId);
                blockedList.Remove(blockedUser);
                blockerUser.BlockedUsers = JsonConvert.SerializeObject(blockedList);
                await _uow.UserRepository.UpdateAsync(blockerUser, blockerUserId);
                await _uow.CommitAsync();
            }

            return true;
        }

        public async Task<List<User>> GetBlockedUserList(int userId)
        {
            var user = await _uow.UserRepository.GetAll()
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var blockedList = (!string.IsNullOrEmpty(user.BlockedUsers))? JsonConvert.DeserializeObject<List<BlockedUsers>>(user.BlockedUsers) : new List<BlockedUsers>();
            var blockIds = blockedList.Select(b => b.BlockedUserId).ToList();

            return await _uow.UserRepository.GetAll()
                .Where(u => blockIds.Contains(u.Id)).ToListAsync();
        }
    }
}