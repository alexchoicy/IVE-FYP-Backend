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
                return BadRequest(response);
            }
            AuthResponeDto data = authServices.login(loginRequestDto);
            if (data == null)
            {
                response.ErrorMessage = "The username or password is incorrect";
                response.Success = false;
                return BadRequest(response);
            }
            response.Data = data;
            return Ok(response); 
        }
    }
}