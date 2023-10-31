using System;

namespace APICore.Common.DTO.Response
{
    public class ReportedUserResponse
    {
        public int Id { get; set; }
        public UserResponse ReportedUser { get; set; }
        public string Coment { get; set; }
        public int ReportStatus { get; set; }
        public DateTime ReportDateTime { get; set; }

    }
}