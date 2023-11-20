namespace APICore.Data.Entities
{
    public class Chat
    {
        public Chat()
        {
            Participants = new HashSet<ChatParticipation>();
            Messages = new HashSet<Message>();
        }
        public int Id { get; set; }

        public string? Name { get; set; }

        public ICollection<ChatParticipation> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}