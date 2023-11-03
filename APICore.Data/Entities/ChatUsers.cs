using APICore.Data.Entities.Enums;

namespace APICore.Data.Entities
{
    public class ChatUsers
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public User From { get; set; }
        public int FromId { get; set; }
        public User To { get; set; }
        public int ToId { get; set; }
        public string Message { get; set; }
        public MessageStatusEnum messageStatus { get; set; }
    }
}