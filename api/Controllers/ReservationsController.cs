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
    [Route("api/v{version:apiVersion}/reservations")]
    public class ReservationsController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetReservations(int? parkingLotId, int? userId)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateReservation()
        {
            return Ok();
        }

        [HttpGet("{reservationId}")]
        public IActionResult GetReservation(int reservationId)
        {
            return Ok(reservationId);
        }

        [HttpPatch("{reservationId}")]
        public IActionResult UpdateReservation(int reservationId)
        {
            return Ok(reservationId);
        }

        [HttpDelete("{reservationId}")]
        public IActionResult DeleteReservation(int reservationId)
        {
            return Ok(reservationId);
        }



    }
}