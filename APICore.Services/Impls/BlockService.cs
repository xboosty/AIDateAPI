using APICore.Data.Entities;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

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
            var existingBlock = await _uow.BlockedUsersRepository.FirstOrDefaultAsync(b => b.BlockerUserId == blockerUserId && b.BlockedUserId == blockedUserId);

            if (existingBlock != null)
            {
                return true;
            }

            var newBlock = new BlockedUsers
            {
                BlockerUserId = blockerUserId,
                BlockedUserId = blockedUserId,
                BlockDateTime = DateTime.UtcNow
            };

            await _uow.BlockedUsersRepository.AddAsync(newBlock);
            await _uow.CommitAsync();

            return true;
        }

        public async Task<bool> UnblockUserAsync(int blockerUserId, int blockedUserId)
        {
            var existingBlock = await _uow.BlockedUsersRepository.FirstOrDefaultAsync(b => b.BlockerUserId == blockerUserId && b.BlockedUserId == blockedUserId) ?? throw new BlockedUserNotFoundException(_localizer);

            _uow.BlockedUsersRepository.Delete(existingBlock);
            await _uow.CommitAsync();

            return true;
        }

        public async Task<List<User>> GetBlockedUserList(int userId)
        {
            var user = await _uow.UserRepository.GetAll()
                .Include(u => u.Blockeds)
                .ThenInclude(b => b.BlockedUser)
                .FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);

            return user.Blockeds.Select(b => b.BlockedUser).ToList();
        }
    }
}