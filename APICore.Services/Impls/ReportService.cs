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
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStringLocalizer<IReportService> _localizer;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ReportService(IUnitOfWork uow, IStringLocalizer<IReportService> localizer, IEmailService emailService, IConfiguration configuration)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
            var reportUserList = await _uow.ReportedUsersRepository.GetAll()
                .Include(r => r.ReporterUser)
                .Include(r => r.ReportedUser)
                            .Where(r => r.ReportedUserId == reportedUserId && r.ReporStatus == ReportStatusEnum.PENDING).ToListAsync();
            var reportsThreshold = int.Parse(_configuration.GetSection("ReportSystemSettings")["ReportsThreshold"]); 
            if (reportUserList.Count() >= reportsThreshold)
                await SendUserReportedNotification(reportUserList);
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

            return reportUsersList.Where(r => r.ReporStatus == (ReportStatusEnum)filter.ReportStatus).ToList();
        }

        public async Task<bool> SetReportStatus(int userId, int reportId, int reportStatus)
        {
            var user = await _uow.UserRepository.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new UserNotFoundException(_localizer);
            var report = await _uow.ReportedUsersRepository.FirstOrDefaultAsync(r => r.Id == reportId);

            if (report != null)
            {
                report.ReporStatus = (ReportStatusEnum)reportStatus;
                await _uow.ReportedUsersRepository.UpdateAsync(report, report.Id);
                await _uow.CommitAsync();
            }

            return true;
        }
        private async Task SendUserReportedNotification(List<ReportedUsers> reportList)
        {
            var msg = HtmlContentGenerator.GenerateReportHtml(reportList);
            var to = _configuration.GetSection("ReportSystemSettings")["AdministratorEmail"];
            await _emailService.SendEmailResponseAsync("Report notification", msg, "david.naranjo@ntsprint.com");
        }

    }
}