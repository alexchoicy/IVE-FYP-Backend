using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.utils;
using System.IdentityModel.Tokens.Jwt;
using api.Models.Respone;

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
            if (httpContextAccessor.HttpContext.User == null)
            {
                response.ErrorMessage = "You are unauthorized";
                response.Success = false;
                return Unauthorized(response);
            }
            string userid = httpContextAccessor.HttpContext.User.getUserID();
            UserResponeDto user = userServices.userInfo(userid);
            response.Data = user;
            return Ok(response);
        }
    }
}