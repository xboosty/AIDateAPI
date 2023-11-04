﻿using System;

namespace APICore.Common.DTO.Response
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public UserResponse To { get; set; }
        public string Message { get; set; }
        public int MessageStatus { get; set; }
        public DateTime Created { get; set; }

    }
}