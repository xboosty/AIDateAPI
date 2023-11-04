namespace APICore.Common.DTO.Request
{
    public class ChatFilterRequest
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int MessageStatus { get; set; }
        public string Message { get; set; }
    }
}