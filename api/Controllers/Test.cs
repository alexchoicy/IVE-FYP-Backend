using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
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

        public Test(NormalDataBaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Entity.User>>> GetUsers()
        {
            utils.ApiResponse<IEnumerable<Models.Entity.User>> response = new utils.ApiResponse<IEnumerable<Models.Entity.User>>();
            try
            {
                response.Data = await _context.User.ToListAsync();
                return Ok(response);
            }
            catch (System.Exception)
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