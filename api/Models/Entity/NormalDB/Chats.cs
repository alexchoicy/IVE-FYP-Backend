using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Websocket.Chat;

namespace api.Models.Entity.NormalDB
{
    public class ChatModel
    {
        [Key]
        public required string chatRoomID { get; set; }
        [ForeignKey("customerID")]
        public required int customerID { get; set; }
        public ChatRoomStatus chatRoomStatus { get; set; }
        public required DateTime createdAt { get; set; }
        public DateTime? endedAt { get; set; }
        public required DateTime updatedAt { get; set; }
        public string? history { get; set; }

        public virtual Users customer { get; set; }
    }

    public enum ChatRoomStatus
    {
        ACTIVE,
        ENDED
    }
}