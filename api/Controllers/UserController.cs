using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.utils;
using System.IdentityModel.Tokens.Jwt;
using api.Models.Respone;
using api.Models.Request;
using api.Exceptions;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string userid = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (userid == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }

                if (httpContextAccessor.HttpContext.User.IsInRole("admin"))
                {
                    StaffReponseDto? staff = userServices.getStaffInfo(userid);
                    return Ok(staff);
                }

                UserResponeDto? user = userServices.getuserInfo(userid);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }

        }

        [HttpGet("users/{userid}")]
        [Authorize]
        public IActionResult GetUserInfo(string userid)
        {
            try
            {
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string tokenUserId = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (tokenUserId == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }
                if (tokenUserId != userid && !httpContextAccessor.HttpContext.User.IsInRole("admin"))
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                UserResponeDto? user = userServices.getuserInfo(userid);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }
        }

        [HttpPatch("users/{userid}")]
        [Authorize]
        public IActionResult UpdateUserInfo(string userid, [FromBody] UserUpdateRequestDto userUpdateRequestDto)
        {
            try
            {
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string tokenUserId = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (tokenUserId == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }
                if (tokenUserId != userid && !httpContextAccessor.HttpContext.User.IsInRole("admin"))
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
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
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }
            catch (DataBaseUpdateException ex)
            {
                return BadRequest(ex);
            }
        }

    }
}