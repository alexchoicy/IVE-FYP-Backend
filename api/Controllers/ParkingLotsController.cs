using api.Models.Entity.NormalDB;
using api.Models.Respone;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    // [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/parkinglots")]
    public class ParkingLotsController : Controller
    {

        private readonly IParkingLotServices parkingLotServices;
        public ParkingLotsController(IParkingLotServices parkingLotServices)
        {
            this.parkingLotServices = parkingLotServices;
        }


        [HttpGet]
        public IActionResult GetParkingLots()
        {

            IEnumerable<ParkingLotReponseDto>? parkingLotReponseDto = parkingLotServices.GetParkingLots();
            if (parkingLotReponseDto == null)
            {
                return NotFound();
            }
            return Ok(parkingLotReponseDto);
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

        [HttpPut("{id}/prices")]
        public IActionResult UpdateParkingLotPrices(int id)
        {
            return Ok();
        }
    }
}