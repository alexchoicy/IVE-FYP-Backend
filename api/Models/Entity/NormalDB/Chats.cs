using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Websocket.Chat;

namespace api.Models.Entity.NormalDB
{
    public class ChatModel
    {
        [Key]
        public required string chatRoomID { get; set; }
        public required int customerID { get; set; }
        public ChatRoomStatus chatRoomStatus { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? endedAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string? history { get; set; }
    }

    public enum ChatRoomStatus
    {
        Active,
        Ended
    }
}