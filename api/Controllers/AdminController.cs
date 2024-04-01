
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
        private readonly IAdminServices adminServices;
        public AdminController(IParkingRecordServices parkingRecordServices, IAdminServices adminServices)
        {
            this.parkingRecordServices = parkingRecordServices;
            this.adminServices = adminServices;
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
        
        [HttpGet("dashboard_data")]
        public IActionResult GetDashboardData()
        {
            AdminDashBoardResponseDto dashboardData = adminServices.GetDashboardData(1);
            return Ok(dashboardData);
        }
        
        [HttpGet("analytics")]
        public async Task< IActionResult> GetAnalytics()    
        {
            AdminAnalyticsResponseDto analyticsData = await adminServices.GetAnalyticsData(1);
            return Ok(analyticsData);
        }
        
        [HttpGet("payment")]
        public async Task<IActionResult> GetPaymentData()
        {
            PaymentAnalytics paymentData = await adminServices.GetPaymentAnalytics(1);
            return Ok(paymentData);
        }
    }
}