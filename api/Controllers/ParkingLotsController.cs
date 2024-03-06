using api.Exceptions;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using api.Services;
using api.utils;
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
            try
            {
                IEnumerable<ParkingLotReponseDto>? parkingLotsData = parkingLotServices.GetParkingLots();
                return Ok(parkingLotsData);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetParkingLot(int id)
        {
            try
            {
                ParkingLotReponseDto? parkingLotData = parkingLotServices.GetParkingLot(id);
                return Ok(parkingLotData);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{id}")]
        public IActionResult UpdateParkingLot(int id, [FromBody] UpdateParkingLotInfoDto updateParkingLotInfoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Invalid model");
                }
                ParkingLotReponseDto updated = parkingLotServices.UpdateParkingLotInfo(id, updateParkingLotInfoDto);
                return Ok(updated);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }

            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/prices/regular")]
        public IActionResult UpdateRegularParkingLotPrices(int id, [FromBody] IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Invalid model");
                }
                ParkingLotReponseDto updated = parkingLotServices.UpdateRegularParkingLotPrices(id, updateParkingLotPricesDto);
                return Ok(updated);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ParkingLotPriceTimesInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (ParkingLotPriceTimeInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (ParkingLotPriceInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/prices/electric")]
        public IActionResult UpdateElectricParkingLotPrices(int id, [FromBody] IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Invalid model");
                }
                ParkingLotReponseDto updated = parkingLotServices.UpdateElectricParkingLotPrices(id, updateParkingLotPricesDto);
                return Ok(updated);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ParkingLotPriceTimesInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (ParkingLotPriceTimeInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (ParkingLotPriceInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
        }
    }
}