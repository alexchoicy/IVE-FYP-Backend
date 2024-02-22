using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/parkinglots")]
    [Authorize]
    public class ParkingLotsController : Controller
    {
        public ParkingLotsController()
        {
        }

        [HttpGet]
        public IActionResult GetParkingLots()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetParkingLot(int id)
        {
            return Ok();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateParkingLot(int id)
        {
            return Ok();
        }
    }
}