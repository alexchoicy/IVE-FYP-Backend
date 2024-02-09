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
    [Route("api/v{version:apiVersion}/ParkingSlot")]
    public class ParkingSlotController : ControllerBase
    {
        private readonly IParkingSlotService _parkingSlot;

        public ParkingSlotController(IParkingSlotService parkingSlotService)
        {
            this._parkingSlot = parkingSlotService;
        }

        [HttpGet("ParkingLotID/{id}")]
        [Authorize]
        public IActionResult GetParkingLots(int id)
        {
            ApiResponse<ParkingLotResponseDto> response = new ApiResponse<ParkingLotResponseDto>();
            try
            {
                response.Data = _parkingSlot.getParkingSlotByID(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.ToString();
                return BadRequest(response);
            }
        }
    }
}