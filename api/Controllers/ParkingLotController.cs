using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.utils;
using System.IdentityModel.Tokens.Jwt;
using api.Models.Respone;
using api.Models.Request;
using api.Exceptions;
using api.Models.Entity.NormalDB;

namespace api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/ParkingLot")]
    public class ParkingLotController : ControllerBase
    {
        private readonly IParkingLotService _parkingLot;

        public ParkingLotController(IParkingLotService parkingLotService)
        {
            this._parkingLot = parkingLotService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Models.Entity.NormalDB.ParkingLot>>> GetParkingLots()
        {
            ApiResponse<IEnumerable<Models.Entity.NormalDB.ParkingLot>> response = new ApiResponse<IEnumerable<Models.Entity.NormalDB.ParkingLot>>();
            try 
            {
                response.Data = await _parkingLot.getALlParkingLot();
                return Ok(response);
            }catch (Exception) 
            {
                response.Success = false;
                response.ErrorMessage = "Error while getting parking slots";
                return BadRequest(response);
            }
        }

        [HttpGet("ParkingLotID/{id}")]
        [Authorize]
        public IActionResult GetParkingLotByID(int id)
        {
            ApiResponse<Models.Entity.NormalDB.ParkingLot> response = new ApiResponse<Models.Entity.NormalDB.ParkingLot>();
            try
            {
                response.Data = _parkingLot.getParkingLotByID(id);
                return Ok(response);
            }
            catch (ParkingLotIDNotExistException)
            {
                response.Success = false;
                response.ErrorMessage = "Parking lot id not exist";
                return BadRequest(response);
            }
        }

    }
}