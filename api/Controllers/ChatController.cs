using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using api.Models.Websocket.Chat;
using api.Services.Chats;
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
            return Ok();
        }
        [Route("{chatRoomId}")]
        public async Task<IActionResult> GetChatRooms(string chatRoomId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
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

                WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected");
                Console.WriteLine(chatRoomId);
                await chatServices.handleConnection(ws, chatRoomId, int.Parse(userID), isAdmin ? UserType.Admin : UserType.User);
                return new EmptyResult();
            }
            else
            {
                // return history of chat
                return Ok();
            }
        }

        [HttpDelete("{chatRoomId}")]
        public async Task<IActionResult> EndChatRoom(string chatRoomId)
        {
            return Ok();
        }

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