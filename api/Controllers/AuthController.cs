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
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Request is invalid");
                }
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
                return StatusCode(StatusCodes.Status429TooManyRequests, ex);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("register")]
        public ActionResult<ApiResponse<AuthResponeDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Request is invalid");
                }
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
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("reset_password")]
        public ActionResult<ApiResponse<string>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Request is invalid");
                }
                authServices.resetPassword(resetPasswordRequestDto);
                //MR.ACCEPTED CAN"T SEND THE MESSAGE SIRRRRRRRRRRRRRRR
                return StatusCode(StatusCodes.Status202Accepted, "Reset password link has been sent to the email. Please check your email to reset your password.");
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("reset_password/veify")]
        public ActionResult<ApiResponse<string>> VerifyResetPassword([FromBody] ResetPasswordVeifyRequestDto resetPasswordVeifyRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Request is invalid");
                }
                authServices.resetPasswordVeify(resetPasswordVeifyRequestDto);
                return Ok("Success");
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("admin/login")]
        public ActionResult<ApiResponse<StaffResponseDto>> AdminLogin([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Request is invalid");
                }
                (StaffResponseDto? data, string token) = authServices.AdminLogin(loginRequestDto);
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });

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
                return StatusCode(StatusCodes.Status429TooManyRequests, ex);
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpPost("logout")]
        public ActionResult<ApiResponse<string>> AdminLogout()
        {
            Response.Cookies.Delete("token");
            return Ok("Logout success");
        }
    }
}