using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/payments")]
    public class PaymentController : ControllerBase
    {
        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetPayments(int? userId, int? parkingRecordId, int? reservationId)
        {
            //admin get all
            //user get only his/her payments
            return Ok();
        }

        [HttpPost]
        //use frombody decide record or reservation payment 
        public IActionResult CreateRecordPayment()
        {
            return Ok();
        }

        [HttpGet("{paymentId}")]
        public IActionResult GetPayment(int paymentId)
        {
            return Ok(paymentId);
        }
    }
}