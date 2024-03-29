using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Services.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    // [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IChatNotifications chatNotifications;

        public NotificationsController(IChatNotifications chatNotifications)
        {
            this.chatNotifications = chatNotifications;
        }

        [Route("chat")]
        public async Task<IActionResult> GetNotifications()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                string userId = HttpContext.Request.Headers["Authorization"].ToString();
                await chatNotifications.HandleConnection(socket);
                return new EmptyResult();
            }

            return Ok();
        }
    }
}