using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class Test : ControllerBase
    {
        private readonly NormalDataBaseContext _context;
        private readonly ITest _test;

        public Test(NormalDataBaseContext context, ITest test)
        {
            _context = context;
            _test = test;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Entity.User>>> GetUsers()
        {
            ApiResponse<IEnumerable<Models.Entity.User>> response = new ApiResponse<IEnumerable<Models.Entity.User>>();
            try
            {
                response.Data = await _test.GetUser();
                return Ok(response);
            }
            catch (Exception)
            {
                response.Success = false;
                response.ErrorMessage = "Error while getting users";
                return BadRequest(response);   
            }
        }

        [HttpGet("/test")]
        public IActionResult test()
        {
            return Ok("test");
        }
    }
}