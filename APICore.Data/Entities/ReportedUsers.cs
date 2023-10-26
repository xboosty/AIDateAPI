using APICore.Data.Entities.Enums;

namespace APICore.Data.Entities
{
    public class ReportedUsers
    {
        public int Id { get; set; }
        public DateTime ReportDateTime { get; set; }
        public User ReporterUser { get; set; }
        public int ReporterUserId { get; set; }
        public User ReportedUser { get; set; }
        public int ReportedUserId { get; set; }
        public string Coment { get; set; }
        public ReportStatusEnum ReporStatus { get; set; }
    }
}