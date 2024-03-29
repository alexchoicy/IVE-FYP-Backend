using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Websocket.Notifications
{
    public class ChatNotificationsDto
    {
        public required string type { get; set; }
        public string roomID { get; set; }
    }

    public enum ChatNotificationType
    {
        NEW_MESSAGE,
        NEW_ROOM,
    }
}