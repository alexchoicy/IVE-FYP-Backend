using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/users/{userId}/vehicles")]
    public class VehiclesController : ControllerBase
    {


        [HttpGet]
        public IActionResult GetVehicles(int userId)
        {
            try
            {
                return Ok(userId);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }
        }

        [HttpPost]
        public IActionResult AddVehicle(int userId)
        {
            return Ok(userId);
        }

        [HttpGet("{vehicleId}")]
        public IActionResult GetVehicle(int userId, int vehicleId)
        {
            return Ok(userId);
        }

        [HttpPatch("{vehicleId}")]
        public IActionResult UpdateVehicle(int userId, int vehicleId)
        {
            return Ok(userId);
        }

        [HttpDelete("{vehicleId}")]
        public IActionResult DeleteVehicle(int userId, int vehicleId)
        {
            return Ok(userId);
        }
    }


}