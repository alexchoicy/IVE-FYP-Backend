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
    [Route("api/v{version:apiVersion}/parkingplans")]
    public class ParkingPlansController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetParkingPlans()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateParkingPlan()
        {
            return Ok();
        }

        [HttpGet("{parkingPlanId}")]
        public IActionResult GetParkingPlan(int parkingPlanId)
        {
            return Ok(parkingPlanId);
        }

        [HttpPatch("{parkingPlanId}")]
        public IActionResult UpdateParkingPlan(int parkingPlanId)
        {
            return Ok(parkingPlanId);
        }

        [HttpDelete("{parkingPlanId}")]
        public IActionResult DeleteParkingPlan(int parkingPlanId)
        {
            return Ok(parkingPlanId);
        }

    }
}