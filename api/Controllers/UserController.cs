using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.utils;
using System.IdentityModel.Tokens.Jwt;
using api.Models.Respone;
using api.Models.Request;
using api.Exceptions;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices userServices;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IUserServices userServices, IHttpContextAccessor httpContextAccessor)
        {
            this.userServices = userServices;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            if (httpContextAccessor.HttpContext?.User == null)
            {
                return Unauthorized("You are unauthorized");
            }
            string userid = httpContextAccessor.HttpContext.User.getUserID() ?? "";
            if (userid == "")
            {
                return Unauthorized("The Token is invalid");
            }
            try
            {
                UserResponeDto? user = userServices.getuserInfo(userid);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }

        }


        [HttpPatch("users/{userid}")]
        [Authorize]
        public IActionResult UpdateUserInfo(string userid, [FromBody] UserUpdateRequestDto userUpdateRequestDto)
        {
            if (httpContextAccessor.HttpContext?.User == null)
            {
                return Unauthorized("You are unauthorized");
            }
            string tokenUserId = httpContextAccessor.HttpContext.User.getUserID() ?? "";
            if (tokenUserId == "")
            {
                return Unauthorized("The Token is invalid");
            }
            if (tokenUserId != userid && !httpContextAccessor.HttpContext.User.IsInRole("admin"))
            {
                return Unauthorized("You are unauthorized");
            }
            try
            {
                UserResponeDto user = userServices.updateUserInfo(userid, userUpdateRequestDto);
                return Accepted(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (InvalidEmailException ex)
            {
                return BadRequest(ex);
            }
            catch (InvalidPhoneNumberException ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("users/{userid}")]
        [Authorize]
        public IActionResult GetUserInfo(string userid)
        {
            if (httpContextAccessor.HttpContext?.User == null)
            {
                return Unauthorized("You are unauthorized");
            }
            string currentUserId = httpContextAccessor.HttpContext.User.getUserID() ?? "";
            if (currentUserId == "")
            {
                return Unauthorized("The Token is invalid");
            }
            try
            {
                UserResponeDto? user = userServices.getuserInfo(userid);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }


        }

    }
}