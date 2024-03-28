using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Websocket.Chat
{
    public class ChatMessage
    {
        public required string message { get; set; }
        public int senderID { get; set; }
        public required string chatSender { get; set; }
        public DateTime createdAt { get; set; }
    }

    public enum ChatSender
    {
        CUSTOMER,
        STAFF,
        SYSTEM
    }
}