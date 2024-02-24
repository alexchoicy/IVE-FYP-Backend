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
            if (!ModelState.IsValid)
            {
                return BadRequest("Request is invalid");
            }
            try
            {
                AuthResponeDto? data = authServices.login(loginRequestDto);
                return Ok(data);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (UserNotActiveException ex)
            {
                return Unauthorized(ex);
            }
            catch (UserLockedException ex)
            {
                return StatusCode(429, ex);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }

        }
        [HttpPost("register")]
        public ActionResult<ApiResponse<AuthResponeDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Request is invalid");
            }
            try
            {
                AuthResponeDto? data = authServices.register(registerRequestDto);
                return Created("", data);
            }
            catch (UserAlreadyExistException ex)
            {
                return Conflict(ex);
            }
            catch (InvalidCredentialsException ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("reset_password")]
        public ActionResult<ApiResponse<string>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Request is invalid");
            }
            try
            {
                authServices.resetPassword(resetPasswordRequestDto);
                //MR.ACCEPTED CAN"T SEND THE MESSAGE SIRRRRRRRRRRRRRRR
                return StatusCode(402, "Reset password link has been sent to the email. Please check your email to reset your password.");
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }

        }

        [HttpPost("reset_password/veify")]
        public ActionResult<ApiResponse<string>> VerifyResetPassword([FromBody] ResetPasswordVeifyRequestDto resetPasswordVeifyRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Request is invalid");
            }
            try
            {
                authServices.resetPasswordVeify(resetPasswordVeifyRequestDto);
                return Ok("Success");
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }

        }

        [HttpPost("admin/login")]
        public ActionResult<ApiResponse<AuthResponeDto>> AdminLogin([FromBody] LoginRequestDto loginRequestDto)
        {
            ApiResponse<AuthResponeDto> response = new ApiResponse<AuthResponeDto>();
            if (!ModelState.IsValid)
            {
                return BadRequest("Request is invalid");
            }
            try
            {
                AuthResponeDto? data = authServices.AdminLogin(loginRequestDto);
                return Ok(data);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (UserNotActiveException ex)
            {
                return Unauthorized(ex);
            }
            catch (UserLockedException ex)
            {
                return StatusCode(429, ex);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }

        }
    }
}