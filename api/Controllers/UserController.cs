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
    [Route("api/v{version:apiVersion}/me")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices userServices;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserController(IUserServices userServices, IHttpContextAccessor httpContextAccessor)
        {
            this.userServices = userServices;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            ApiResponse<UserResponeDto> response = new ApiResponse<UserResponeDto>();
            if (httpContextAccessor.HttpContext?.User == null)
            {
                response.ErrorMessage = "You are unauthorized";
                response.Success = false;
                return Unauthorized(response);
            }
            string userid = httpContextAccessor.HttpContext.User.getUserID() ?? "";
            if (userid == "")
            {
                response.ErrorMessage = "Some Error Occured, Please try again later.";
                response.Success = false;
                return BadRequest(response);
            }
            UserResponeDto? user = userServices.getuserInfo(userid);
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.Success = false;
                return NotFound(response);
            }
            response.Data = user;
            return Ok(response);
        }

        [HttpPatch]
        [Authorize]
        public IActionResult UpdateUserInfo([FromBody] UserUpdateRequestDto userUpdateRequestDto)
        {
            ApiResponse<UserResponeDto> response = new ApiResponse<UserResponeDto>();
            if (httpContextAccessor.HttpContext?.User == null)
            {
                response.ErrorMessage = "You are unauthorized";
                response.Success = false;
                return Unauthorized(response);
            }
            string userid = httpContextAccessor.HttpContext.User.getUserID() ?? "";
            if (userid == "")
            {
                response.ErrorMessage = "Some Error Occured, Please try again later.";
                response.Success = false;
                return BadRequest(response);
            }
            try
            {
                
                UserResponeDto user = userServices.updateUserInfo(userid, userUpdateRequestDto);
                response.Data = user;
                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return NotFound(response);
            }
            catch (InvalidEmailException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }
            catch (InvalidPhoneNumberException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }
        }
    }
}