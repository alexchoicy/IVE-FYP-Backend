using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Respone;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/parkinglots/{parkingLotId}/parkingspaces")]
    public class ParkingSpacesController : ControllerBase
    {
        private readonly ParkingLotServices parkingLotServices;
        public ParkingSpacesController(ParkingLotServices parkingLotServices)
        {
            this.parkingLotServices = parkingLotServices;
        }

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