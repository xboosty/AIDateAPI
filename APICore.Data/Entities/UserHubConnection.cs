using System;

namespace APICore.Data.Entities
{
    public class UserHubConnection
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}