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
    [Route("api/v{version:apiVersion}/parkinglots/{parkingLotId}/parkingspaces")]
    public class ParkingSpacesController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetParkingSpaces(int parkingLotId)
        {
            return Ok(parkingLotId);
        }

        [HttpGet("{parkingSpaceId}")]
        public IActionResult GetParkingSpace(int parkingLotId, int parkingSpaceId)
        {
            return Ok(parkingLotId);
        }

        [HttpPatch("{parkingSpaceId}")]
        public IActionResult UpdateParkingSpace(int parkingLotId, int parkingSpaceId)
        {
            return Ok(parkingLotId);
        }
    }
}