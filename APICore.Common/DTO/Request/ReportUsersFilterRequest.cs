namespace APICore.Common.DTO.Request
{
    public class ReportUsersFilterRequest
    {
        public int ReporterUserId { get; set; }
        public int ReportedUserId { get; set; }
        public int ReportStatus { get; set; }
        public string Coment { get; set; }
    }
}