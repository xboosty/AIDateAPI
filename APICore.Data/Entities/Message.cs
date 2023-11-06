using APICore.Data.Entities.Enums;

namespace APICore.Data.Entities
{
    public class Message
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime SentDate { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public MessageStatusEnum Status { get; set; }
    }
}