using System;

namespace APICore.Common.DTO.Response
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public UserResponse Sender { get; set; }
        public string Message { get; set; }
        public int MessageStatus { get; set; }
        public DateTime Created { get; set; }

    }
}