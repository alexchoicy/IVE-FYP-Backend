using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using api.Models.Respone;
using api.Models.Websocket.Chat;
using api.Services.Chats;
using api.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/chats")]
    public class ChatController : ControllerBase
    {

        private readonly IChatServices chatServices;
        private readonly IConfiguration _configuration;

        public ChatController(IChatServices chatServices, IConfiguration configuration)
        {
            this.chatServices = chatServices;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatRoom()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            (string userID, bool isAdmin) = ValidateTokenAndReturnID(token);

            if (string.IsNullOrEmpty(userID))
            {
                return Unauthorized();
            }
            string roomKey = await chatServices.CreateChatRoom(int.Parse(userID));
            return Ok(roomKey);
        }

        [HttpGet]
        public async Task<IActionResult> GetChatRooms(int page = 1, int recordsPerPage = 15)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            (string userID, bool isAdmin) = ValidateTokenAndReturnID(token);

            if (string.IsNullOrEmpty(userID))
            {
                return Unauthorized();
            }

            PagedResponse<IEnumerable<ChatResponseDto>> rooms = await chatServices.GetChatRooms(int.Parse(userID), isAdmin, page, recordsPerPage);
            return Ok(rooms);
        }

        [Route("{chatRoomId}")]
        public async Task<IActionResult> GetChatRooms(string chatRoomId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            (string userID, bool isAdmin) = ValidateTokenAndReturnID(token);

            if (string.IsNullOrEmpty(userID))
            {
                return Unauthorized();
            }
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {


                WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");
                Console.WriteLine(chatRoomId);
                await chatServices.handleConnection(ws, chatRoomId, int.Parse(userID), isAdmin ? ChatSender.Staff : ChatSender.Customer);
                return new EmptyResult();
            }
            else
            {
                // int count = chatServices.GetCurrentRoomCount();
                // return Ok(count);
                return BadRequest();
            }
        }

        [HttpGet("{chatRoomId}/history")]
        public async Task<IActionResult> GetChatRoomHistory(string chatRoomId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            (string userID, bool isAdmin) = ValidateTokenAndReturnID(token);

            if (string.IsNullOrEmpty(userID))
            {
                return Unauthorized();
            }

            ICollection<ChatMessage> history = await chatServices.GetChatHistory(chatRoomId, int.Parse(userID));
            return Ok(history);
        }

        [HttpGet("{chatRoomId}/members")]
        public IActionResult GetChatRoomMembers(string chatRoomId)
        {
            int members = chatServices.GetRoomMember(chatRoomId);
            return Ok(members);
        }

        // [HttpDelete("{chatRoomId}")]
        // public async Task<IActionResult> EndChatRoom(string chatRoomId)
        // {
        //     return Ok();
        // }

        private (string?, bool) ValidateTokenAndReturnID(string token)
        {
            try
            {
                token = token.Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                Console.WriteLine(jwtToken);
                return (jwtToken.Claims.First(x => x.Type == "nameid").Value, jwtToken.Claims.First(x => x.Type == "role").Value == "admin");
            }
            catch
            {
                return (null, false);
            }
        }
    }
}