namespace APICore.Data.Entities
{
    public class BlockedUsers
    {
        public int Id { get; set; }
        public DateTime BlockDateTime { get; set; }
        public User BlockerUser { get; set; }
        public int BlockerUserId { get; set; }
        public User BlockedUser { get; set; }
        public int BlockedUserId { get; set; }
    }
}