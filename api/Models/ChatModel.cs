using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class ChatModel
    {
        //the socketKey 
        public required string ChatRoomID { get; set; }
        public required string customerID { get; set; }
        public required string customerName { get; set; }
        public ChatRoomStatus chatRoomStatus { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime endedAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string Histroy { get; set; } //should be save a json of ChatHistory
    }

    public enum ChatRoomStatus
    {
        Active,
        Ended
    }

}