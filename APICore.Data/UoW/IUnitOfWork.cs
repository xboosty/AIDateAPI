using APICore.Data.Entities;
using APICore.Data.Repository;
using System;
using System.Threading.Tasks;

namespace APICore.Data.UoW
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> UserRepository { get; set; }
        IGenericRepository<UserToken> UserTokenRepository { get; set; }
        IGenericRepository<Setting> SettingRepository { get; set; }
        IGenericRepository<Log> LogRepository { get; set; }
        IGenericRepository<BlockedUsers> BlockedUsersRepository { get; set; }
        IGenericRepository<ReportedUsers> ReportedUsersRepository { get; set; }
        IGenericRepository<Message> MessageRepository { get; set; }
        IGenericRepository<Chat> ChatRepository { get; set; }
        IGenericRepository<ChatParticipation> ChatParticipationRepository { get; set; }
        IGenericRepository<UserHubConnection> HubConnectionRepository { get; set; }

        Task<int> CommitAsync();
    }
}