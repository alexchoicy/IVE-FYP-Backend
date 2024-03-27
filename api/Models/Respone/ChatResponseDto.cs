using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Websocket.Chat;

namespace api.Models.Respone
{
    public class ChatResponseDto
    {
        public string chatRoomID { get; set; }
        public int customerID { get; set; }
        public required string customerName { get; set; }
        public string chatRoomStatus { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? endedAt { get; set; }
        public DateTime updatedAt { get; set; }
        public IEnumerable<ChatMessage>? history { get; set; }
    }
}