using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/parkinglots/{parkingLotId}/parkingspaces/{parkingSpaceId}/parkingspaceplans")]
    public class ParkingSpacePlansController : ControllerBase
    {
        [HttpGet("history")]
        public IActionResult GetParkingSpacePlansHistory(int parkingLotId, int parkingSpaceId)
        {
            return Ok(parkingLotId);
        }

        [HttpPost]
        public IActionResult AddParkingSpacePlan(int parkingLotId, int parkingSpaceId)
        {
            return Ok(parkingLotId);
        }

        [HttpGet("{parkingSpacePlanId}")]
        public IActionResult GetParkingSpacePlan(int parkingLotId, int parkingSpaceId, int parkingSpacePlanId)
        {
            return Ok(parkingLotId);
        }

        [HttpPost("{parkingSpacePlanId}")]
        public IActionResult UpdateParkingSpacePlan(int parkingLotId, int parkingSpaceId, int parkingSpacePlanId)
        {
            return Ok(parkingLotId);
        }

        [HttpDelete("{parkingSpacePlanId}")]
        public IActionResult DeleteParkingSpacePlan(int parkingLotId, int parkingSpaceId, int parkingSpacePlanId)
        {
            return Ok(parkingLotId);
        }

    }
}