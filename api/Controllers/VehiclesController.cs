using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/users/{userId}/vehicles")]
    public class VehiclesController : ControllerBase
    {


        [HttpGet]
        public IActionResult GetVehicles(int userId)
        {
            return Ok(userId);
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