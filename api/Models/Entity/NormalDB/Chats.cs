using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Websocket.Chat;

namespace api.Models.Entity.NormalDB
{
    public class Chats
    {
        [Key]
        public string chatRoomID { get; set; }
        public string userID { get; set; }
        public ChatRoomStatus chatRoomStatus { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime endedAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string History { get; set; }
    }

    public class ChatHistory
    {
        public string message { get; set; }
        public string sender { get; set; }
        public ChatSender chatSender { get; set; }
        public DateTime createdAt { get; set; }
    }
}