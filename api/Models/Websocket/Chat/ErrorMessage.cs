using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Websocket.Chat
{
    public class WebSocketErrorMessage
    {
        public string message { get; set; }
        public WebSocketErrorType error { get; set; }
        public DateTime createdAt { get; set; }
    }

    public enum WebSocketErrorType
    {
        DUPLICATED_CONNECTION,
        CHATROOM_NOT_FOUND,
        UNAUTHORIZED,
        CHATROOM_ENDED,
        USER_EXIT_SUCCESSFULLY,
    }
}