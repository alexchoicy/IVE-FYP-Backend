using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using api.Models;
using api.Models.Websocket.Chat;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.Chats
{
    public interface IChatServices
    {
        Task<string> CreateChatRoom();
        Task handleConnection(WebSocket socket, string roomKey, int userID, UserType userType);
        Task ReceiveMessage(WebSocket socket, string roomKey, int userID, UserType userType);
        Task SendMessage(string roomKey, string message, bool isToAll, int? sourceID = null, UserType? sourceUserType = null);
        void EndChatRoom(string roomKey);
        Task ExitChatRoom(string roomKey, int userID);
    }
    public class ChatServices : IChatServices
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan timeout = TimeSpan.FromMinutes(1);
        //first string is a chatRoomId
        private static ConcurrentDictionary<string, IList<ChatUser>> chatRoom = new ConcurrentDictionary<string, IList<ChatUser>>();
        public ChatServices(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public async Task<string> CreateChatRoom()
        {
            return "";
        }

        public async Task handleConnection(WebSocket socket, string roomKey, int userID, UserType userType)
        {
            Console.WriteLine("roomKey: " + roomKey + " userID: " + userID + " userType: " + userType);
            Console.WriteLine("chatRoom: " + chatRoom.Count);
            if (!chatRoom.ContainsKey(roomKey))
            {
                chatRoom.TryAdd(roomKey, new List<ChatUser>());
            }

            foreach (var user in chatRoom[roomKey])
            {
                if (user.UserId == userID && user.UserType == userType)
                {
                    await user.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "A new client connect to the chat", CancellationToken.None);
                    chatRoom[roomKey].Remove(user);
                    break;
                }
            }

            ChatUser chatUser = new ChatUser
            {
                UserId = userID,
                UserType = userType,
                WebSocket = socket
            };

            chatRoom[roomKey].Add(chatUser);

            await SendMessage(roomKey, "User " + userID + " joined the chat", true);

            await ReceiveMessage(socket, roomKey, userID, userType);

            Console.WriteLine(socket.State);
        }

        public async Task ReceiveMessage(WebSocket socket, string roomKey, int userID, UserType userType)
        {
            byte[] buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            while (socket.State == WebSocketState.Open)
            {
                try
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine(socket.State);
                    // await EndChatRoom(roomKey);
                    socket.Dispose();
                    chatRoom[roomKey].Remove(chatRoom[roomKey].FirstOrDefault(x => x.UserId == userID));
                    throw;
                }
                catch (WebSocketException)
                {
                    Console.WriteLine("WebSocket connection closed abruptly.");
                    await ExitChatRoom(roomKey, userID);
                    throw;
                }

                if (result.EndOfMessage && result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine(message);
                    await SendMessage(roomKey, message, false, userID, userType);
                }
            }

            Console.WriteLine("connection closed");
            await ExitChatRoom(roomKey, userID);
        }

        public async Task SendMessage(string roomKey, string message, bool isToAll, int? sourceID = null, UserType? sourceUserType = null)
        {
            Console.WriteLine("user in room: " + chatRoom[roomKey].Count);
            foreach (var chatUser in chatRoom[roomKey])
            {
                if (isToAll || (chatUser.UserId != sourceID && chatUser.UserType == sourceUserType))
                {
                    await chatUser.WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public void EndChatRoom(string roomKey)
        {
            if (chatRoom[roomKey].Count == 0)
            {
                chatRoom.TryRemove(roomKey, out _);
            }
        }

        public async Task ExitChatRoom(string roomKey, int userID)
        {
            foreach (var chatUser in chatRoom[roomKey])
            {
                if (chatUser.UserId == userID)
                {
                    await SendMessage(roomKey, "User " + userID + " left the chat", true);

                    if (chatUser.WebSocket.State == WebSocketState.Open)
                    {
                        await chatUser.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User left the chat", CancellationToken.None);
                    }
                    EndChatRoom(roomKey);
                    chatUser.WebSocket.Dispose();
                    chatRoom[roomKey].Remove(chatUser);
                    break;
                }
            }
        }
    }
}