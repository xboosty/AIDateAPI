using APICore.Common.DTO.Request;
using APICore.Data.Entities;

namespace APICore.Services
{
    public interface IReportService
    {
        Task<bool> ReportUserAsync(int reporterUserId, int reportedUserId, string coment);
        Task<List<ReportedUsers>> GetReportedUserList(ReportUsersFilterRequest filter);
        Task<bool> SetReportStatus(int userId, int reportId, int reportStatus);
    }
}