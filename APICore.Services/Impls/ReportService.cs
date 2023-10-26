using APICore.Common.DTO.Request;
using APICore.Data.Entities;
using APICore.Data.Entities.Enums;
using APICore.Data.UoW;
using APICore.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace APICore.Services.Impls
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IReportService> _localizer;

        public ReportService(IUnitOfWork uow, IStringLocalizer<IReportService> localizer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<bool> ReportUserAsync(int reporterUserId, int reportedUserId, string coment)
        {
            var reporterUser = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == reporterUserId) ?? throw new UserNotFoundException(_localizer);
            var reportedUser = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == reportedUserId) ?? throw new UserNotFoundException(_localizer);
            var existingReport = await _uow.ReportedUsersRepository.FirstOrDefaultAsync(r => r.ReporterUserId == reporterUserId && r.ReportedUserId == reportedUserId);

            if (existingReport != null)
            {
                return true;
            }

            var newReport = new ReportedUsers
            {
                ReporterUserId = reporterUserId,
                ReportedUserId = reportedUserId,
                ReportDateTime = DateTime.UtcNow,
                Coment = coment,
                ReporStatus = Data.Entities.Enums.ReportStatusEnum.PENDING
            };

            await _uow.ReportedUsersRepository.AddAsync(newReport);
            await _uow.CommitAsync();

            return true;
        }

        public async Task<List<ReportedUsers>> GetReportedUserList(ReportUsersFilterRequest filter)
        {
            var reportUsersList = _uow.ReportedUsersRepository.GetAll()
                .Include(r => r.ReportedUser)
                .AsQueryable();

            if (filter.ReportedUserId > 0)
                reportUsersList = reportUsersList.Where(r => r.ReportedUserId == filter.ReportedUserId);

            if (filter.ReporterUserId > 0)
                reportUsersList = reportUsersList.Where(r => r.ReporterUserId == filter.ReporterUserId);

            if (!string.IsNullOrEmpty(filter.Coment))
                reportUsersList = reportUsersList.Where(r => r.Coment.Contains(filter.Coment));

            return reportUsersList.Where(r => r.ReporStatus == (ReportStatusEnum) filter.ReporStatus).ToList();    
        }

        public async Task<bool> SetReportStatus(int userId, int reportId, int reportStatus)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var report = await _uow.ReportedUsersRepository.FirstOrDefaultAsync(r => r.Id == reportId);

            if (report != null)
            {
                report.ReporStatus = (ReportStatusEnum)reportStatus;
                await _uow.ReportedUsersRepository.UpdateAsync(report,report.Id);
                await _uow.CommitAsync();
            }

            return true;
        }
    }
}