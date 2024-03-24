
using api.Exceptions;
using api.Models.Respone;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "admin")]
    [Route("api/v{version:apiVersion}/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IParkingRecordServices parkingRecordServices;

        public AdminController(IParkingRecordServices parkingRecordServices)
        {
            this.parkingRecordServices = parkingRecordServices;
        }

        [HttpGet("parkingrecords/{sessionid}")]
        public async Task<IActionResult> GetParkingRecord(int sessionid)
        {
            try
            {
                ParkingRecordResponseDtoDetailed parkingRecordData = await parkingRecordServices.GetParkingRecordAdmin(sessionid);
                return Ok(parkingRecordData);
            }
            catch (ParkingRecordNotFoundException ex)
            {
                return NotFound(ex);
            }
        }
    }
}