using api.Models.Request;
using api.Models.Respone;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices authServices;
        private readonly IUserServices userServices;

        public AuthController(IAuthServices authServices, IUserServices userServices)
        {
            this.authServices = authServices;
            this.userServices = userServices;
        }

        [HttpPost("login")]
        public ActionResult<ApiResponse<AuthResponeDto>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (loginRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                return BadRequest(response);
            }
            if (!userServices.IsUserExists(loginRequestDto.userName))
            {
                response.ErrorMessage = "The User is not exit";
                response.Success = false;
                return NotFound(response);
            }
            if (!userServices.isUserActive(loginRequestDto.userName))
            {
                response.ErrorMessage = "The User is not active";
                response.Success = false;
                return Unauthorized(response);
            }
            if (userServices.IsUserLockedOut(loginRequestDto.userName))
            {
                var lockoutEnd = userServices.GetLockoutEndDate(loginRequestDto.userName);
                var remainingTime = lockoutEnd - DateTime.Now;
                response.ErrorMessage = $"Your account is locked. Please try again in {Math.Round(remainingTime.TotalMinutes)} minutes.";
                response.Success = false;
                return Unauthorized(response);
            }
            AuthResponeDto? data = authServices.login(loginRequestDto);
            if (data == null)
            {
                response.ErrorMessage = "The username or password is incorrect";
                response.Success = false;
                return Unauthorized(response);
            }
            response.Data = data;
            return Ok(response);
        }
    }
}