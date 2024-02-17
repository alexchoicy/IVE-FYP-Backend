using api.Models.Request;
using api.Models.Respone;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Mvc;
using api.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices authServices;

        public AuthController(IAuthServices authServices)
        {
            this.authServices = authServices;
        }

        [HttpPost("login")]
        public ActionResult<ApiResponse<AuthResponeDto>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (loginRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                response.StatusCode = 400;
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
                response.StatusCode = 404;
                return NotFound(response);
            }
            catch (UserNotActiveException ex)
            {
                response.ErrorMessage = ex.Message;
                response.StatusCode = 401;
                response.Success = false;
                return Unauthorized(response);
            }
            catch (UserLockedException ex)
            {
                response.ErrorMessage = ex.Message;
                response.StatusCode = 401;
                response.Success = false;
                return Unauthorized(response);
            }
            catch (InvalidCredentialsException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 401;
                return Unauthorized(response);
            }
        }
        [HttpPost("register")]
        public ActionResult<ApiResponse<AuthResponeDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (registerRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                response.StatusCode = 400;
                return BadRequest(response);
            }
            try
            {
                AuthResponeDto? data = authServices.register(registerRequestDto);
                response.Data = data;
                response.StatusCode = 201;
                return Created("", response);
            }
            catch (UserAlreadyExistException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 409;
                return Conflict(response);
            }
            catch (InvalidCredentialsException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 400;
                return BadRequest(response);
            }
        }

        [HttpPost("reset_password")]
        public ActionResult<ApiResponse<string>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (resetPasswordRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                response.StatusCode = 400;
                return BadRequest(response);
            }
            try
            {
                authServices.resetPassword(resetPasswordRequestDto);
                response.Data = "Reset password link has been sent to the email. Please check your email to reset your password.";
                response.StatusCode = 202;
                return Accepted(response);
            }
            catch (UserNotFoundException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 404;
                return NotFound(response);
            }
        }

        [HttpPost("reset_password/veify")]
        public ActionResult<ApiResponse<string>> VerifyResetPassword([FromBody] ResetPasswordVeifyRequestDto resetPasswordVeifyRequestDto)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            if (resetPasswordVeifyRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                response.StatusCode = 400;
                return BadRequest(response);
            }
            try
            {
                authServices.resetPasswordVeify(resetPasswordVeifyRequestDto);
                response.Data = "Success";
                return Ok(response);
            }
            catch (InvalidCredentialsException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 401;
                return Unauthorized(response);
            }
        }

        [HttpPost("admin/login")]
        public ActionResult<ApiResponse<AuthResponeDto>> AdminLogin([FromBody] LoginRequestDto loginRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (loginRequestDto == null)
            {
                response.ErrorMessage = "Request is null";
                response.Success = false;
                response.StatusCode = 400;
                return BadRequest(response);
            }
            try
            {
                AuthResponeDto? data = authServices.AdminLogin(loginRequestDto);
                response.Data = data;
                return Ok(response);
            }
            catch (UserNotFoundException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 404;
                return NotFound(response);
            }
            catch (UserNotActiveException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 401;
                return Unauthorized(response);
            }
            catch (UserLockedException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 401;
                return Unauthorized(response);
            }
            catch (InvalidCredentialsException ex)
            {
                response.ErrorMessage = ex.Message;
                response.Success = false;
                response.StatusCode = 401;
                return Unauthorized(response);
            }
        }
    }
}