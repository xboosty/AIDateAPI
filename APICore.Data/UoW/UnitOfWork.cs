using APICore.Data.Entities;
using APICore.Data.Repository;
using System;
using System.Threading.Tasks;

namespace APICore.Data.UoW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly CoreDbContext _context;

        public UnitOfWork(CoreDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            UserRepository ??= new GenericRepository<User>(_context);
            UserTokenRepository ??= new GenericRepository<UserToken>(_context);
            SettingRepository ??= new GenericRepository<Setting>(_context);
            LogRepository ??= new GenericRepository<Log>(_context);
            BlockedUsersRepository ??= new GenericRepository<BlockedUsers>(_context);
            ReportedUsersRepository ??= new GenericRepository<ReportedUsers>(_context);
            MessageRepository ??= new GenericRepository<Message>(context);
            ChatRepository ??= new GenericRepository<Chat>(context);
            ChatParticipationRepository ??= new GenericRepository<ChatParticipation>(context);
            HubConnectionRepository ??= new GenericRepository<UserHubConnection>(_context);
        }

        public IGenericRepository<User> UserRepository { get; set; }
        public IGenericRepository<UserToken> UserTokenRepository { get; set; }
        public IGenericRepository<Setting> SettingRepository { get; set; }
        public IGenericRepository<Log> LogRepository { get; set; }
        public IGenericRepository<BlockedUsers> BlockedUsersRepository { get; set; }
        public IGenericRepository<ReportedUsers> ReportedUsersRepository { get; set; }
        public IGenericRepository<Message> MessageRepository { get; set; }
        public IGenericRepository<Chat> ChatRepository { get; set; }
        public IGenericRepository<ChatParticipation> ChatParticipationRepository { get; set; }
        public IGenericRepository<UserHubConnection> HubConnectionRepository { get; set; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}