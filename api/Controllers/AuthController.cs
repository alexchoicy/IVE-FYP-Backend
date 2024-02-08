using api.Models.Request;
using api.Models.Respone;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Mvc;
using static api.Exceptions.AuthException;

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
            try
            {
                AuthResponeDto? data = authServices.login(loginRequestDto);
                response.Data = data;
                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return NotFound(response);
            }
            catch (UserNotActiveException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return Unauthorized(response);
            }
            catch (UserLockedException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return Unauthorized(response);
            }
            catch (InvalidCredentialsException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return Unauthorized(response);
            }
        }
        public ActionResult<ApiResponse<AuthResponeDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (registerRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                return BadRequest(response);
            }
            try
            {
                AuthResponeDto? data = authServices.register(registerRequestDto);
                response.Data = data;
                return Ok(response);
            }
            catch (UserAlreadyExistException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                return BadRequest(response);
            }
        }
    }
}