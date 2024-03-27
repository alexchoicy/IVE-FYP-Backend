using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace api.Models.Websocket.Chat
{
    public class ChatUser
    {
        public required int UserId { get; set; }
        public required ChatSender UserType { get; set; }
        public required WebSocket WebSocket { get; set; }
    }

}