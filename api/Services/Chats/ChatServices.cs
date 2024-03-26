using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Websocket.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

//hi why we have usertype and senderType what i doing
namespace api.Services.Chats
{
    public interface IChatServices
    {
        Task<string> CreateChatRoom(int customerID);
        Task handleConnection(WebSocket socket, string roomKey, int userID, UserType userType);
        Task ReceiveMessage(WebSocket socket, string roomKey, int userID, UserType userType);
        Task SendMessage(string roomKey, string message, bool isToAll, int? sourceID = null, UserType? sourceUserType = null);
        void EndChatRoom(string roomKey);
        Task ExitChatRoom(string roomKey, int userID, UserType userType);
        int GetCurrentRoomCount();
        Task<ICollection<ChatMessage>> GetChatHistory(string roomKey, int userID);
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

        public async Task<string> CreateChatRoom(int customerID)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                do
                {
                    string roomKey = Guid.NewGuid().ToString();
                    ChatModel? chat = normalDataBaseContext.Chats.FirstOrDefault(x => x.chatRoomID == roomKey);
                    if (chat != null)
                    {
                        continue;
                    }
                    ChatModel newChat = new ChatModel
                    {
                        chatRoomID = roomKey,
                        customerID = customerID,
                        chatRoomStatus = ChatRoomStatus.Active,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    };
                    normalDataBaseContext.Chats.Add(newChat);
                    await normalDataBaseContext.SaveChangesAsync();

                    chatRoom.TryAdd(roomKey, new List<ChatUser>());
                    return roomKey;
                } while (true);
            }

        }

        public async Task handleConnection(WebSocket socket, string roomKey, int userID, UserType userType)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                Console.WriteLine(roomKey);
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ChatModel chat = normalDataBaseContext.Chats.FirstOrDefault(x => x.chatRoomID == roomKey);
                if (chat == null)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Chat room not found", CancellationToken.None);
                    return;
                }

                if (userType != UserType.Admin && chat.customerID != userID)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Unauthorized", CancellationToken.None);
                    return;
                }
                if (chat.chatRoomStatus == ChatRoomStatus.Ended)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Chat room has ended", CancellationToken.None);
                    return;
                }
                else
                {
                    if (!chatRoom.ContainsKey(roomKey))
                    {
                        chatRoom.TryAdd(roomKey, new List<ChatUser>());
                    }
                }
            }

            var userToKickOut = chatRoom[roomKey].FirstOrDefault(user => user.UserId == userID && user.UserType == userType);

            if (userToKickOut != null)
            {
                Console.WriteLine("Duplicate connection" + roomKey + " " + userID + " " + userType);
                await userToKickOut.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "A new client connect to the chat", CancellationToken.None);
                chatRoom[roomKey].Remove(userToKickOut);
            }

            ChatUser chatUser = new ChatUser
            {
                UserId = userID,
                UserType = userType,
                WebSocket = socket
            };

            chatRoom[roomKey].Add(chatUser);

            await SendMessage(roomKey, userType.ToString() + userID + " joined the chat", true);

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
                    Console.WriteLine("Exit chat room" + roomKey + " " + userID + " " + userType);
                    await ExitChatRoom(roomKey, userID, userType);
                    throw;
                }

                if (result.EndOfMessage && result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    SaveMessage(roomKey, message, userID, userType == UserType.Admin ? ChatSender.Staff : ChatSender.Customer);
                    Console.WriteLine(message);
                    await SendMessage(roomKey, message, false, userID, userType);
                }
            }

            Console.WriteLine("connection closed");

            Console.WriteLine("Exit chat room" + roomKey + " " + userID + " " + userType + " " + socket.State);
            await ExitChatRoom(roomKey, userID, userType);
        }

        public async Task SendMessage(string roomKey, string message, bool isToAll, int? sourceID = null, UserType? sourceUserType = null)
        {
            Console.WriteLine("user in room: " + chatRoom[roomKey].Count);
            for (int i = 0; i < chatRoom[roomKey].Count; i++)
            {
                ChatUser chatUser = chatRoom[roomKey][i];
                Console.WriteLine(chatRoom[roomKey][i].UserId);
                if ((isToAll || (chatUser.UserId != sourceID && chatUser.UserType == sourceUserType)) && chatUser.WebSocket.State == WebSocketState.Open)
                {
                    await chatUser.WebSocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task SaveMessage(string roomKey, string message, int sender, ChatSender senderType)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ChatModel chat = normalDataBaseContext.Chats.FirstOrDefault(x => x.chatRoomID == roomKey);
                if (chat == null)
                {
                    return;
                }
                ICollection<ChatMessage> chatHistories;

                if (chat.history == null)
                {
                    chatHistories = new List<ChatMessage>();
                }
                else
                {
                    chatHistories = JsonConvert.DeserializeObject<ICollection<ChatMessage>>(chat.history);
                }

                ChatMessage chatHistory = new ChatMessage
                {
                    message = message,
                    senderID = sender,
                    chatSender = senderType,
                    createdAt = DateTime.Now
                };

                chatHistories.Add(chatHistory);
                chat.updatedAt = DateTime.Now;
                chat.history = JsonConvert.SerializeObject(chatHistories);
                await normalDataBaseContext.SaveChangesAsync();
            }
        }

        public void EndChatRoom(string roomKey)
        {

            chatRoom.TryRemove(roomKey, out _);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ChatModel chat = normalDataBaseContext.Chats.FirstOrDefault(x => x.chatRoomID == roomKey);
                if (chat == null)
                {
                    return;
                }
                chat.chatRoomStatus = ChatRoomStatus.Ended;
                chat.endedAt = DateTime.Now;
                normalDataBaseContext.SaveChanges();
            }

        }

        public async Task ExitChatRoom(string roomKey, int userID, UserType userType)
        {
            foreach (var chatUser in chatRoom[roomKey])
            {
                if (chatUser.UserId == userID && chatUser.UserType == userType)
                {
                    chatUser.WebSocket.Dispose();
                    chatRoom[roomKey].Remove(chatUser);
                    if (chatRoom[roomKey].Count == 0)
                    {
                        EndChatRoom(roomKey);
                    }
                    else
                    {
                        await SendMessage(roomKey, "User " + userID + " left the chat", true);
                    }
                    break;
                }
            }
        }

        public int GetCurrentRoomCount()
        {
            return chatRoom.Count;
        }

        public async Task<ICollection<ChatMessage>> GetChatHistory(string roomKey, int userID)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ChatModel chat = await normalDataBaseContext.Chats.FirstOrDefaultAsync(x => x.chatRoomID == roomKey);
                ICollection<ChatMessage> chatHistories;

                if (chat.history == null || chat == null)
                {
                    chatHistories = new List<ChatMessage>();
                }
                else
                {
                    chatHistories = JsonConvert.DeserializeObject<ICollection<ChatMessage>>(chat.history);
                }

                return chatHistories;
            }
        }
    }
}