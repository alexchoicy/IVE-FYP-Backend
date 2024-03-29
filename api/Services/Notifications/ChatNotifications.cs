using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using api.Models.Websocket.Notifications;
using Newtonsoft.Json;

namespace api.Services.Notifications
{
    public interface IChatNotifications
    {
        Task HandleConnection(WebSocket socket);
        void RemoveUser(string token);
    }

    public class ChatNotifications : IChatNotifications
    {
        private static readonly ConcurrentDictionary<string, WebSocket> users = new ConcurrentDictionary<string, WebSocket>();

        public void RemoveUser(string token)
        {
            users.TryRemove(token, out _);
        }


        public async Task HandleConnection(WebSocket socket)
        {
            string token = Guid.NewGuid().ToString();
            users.TryAdd(token, socket);
            Console.WriteLine($"Total users connected: {users.Count}");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

            RemoveUser(token);
        }

        public static async Task SendMessage(string roomID, ChatNotificationType type)
        {
            ChatNotificationsDto nootifcation = new ChatNotificationsDto
            {
                type = type.ToString(),
                roomID = roomID
            };

            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(nootifcation));
            foreach (var user in users)
            {
                if (user.Value.State == WebSocketState.Open)
                {
                    await user.Value.SendAsync(new ArraySegment<byte>(message, 0, message.Length), WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);
                }
            }
        }
    }
}