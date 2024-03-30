using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentServices paymentServices;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PaymentController(IPaymentServices paymentServices, IHttpContextAccessor httpContextAccessor)
        {
            this.paymentServices = paymentServices;
            this.httpContextAccessor = httpContextAccessor;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetPayments(int? userId, int? parkingRecordId, int? reservationId)
        {
            //admin get all
            //user get only his/her payments
            if (userId != null)
            {
                IEnumerable<PaymentResponseDto> payments = paymentServices.GetPaymentsByUserId(userId.Value);
                return Ok(payments);
            }
            else if (parkingRecordId != null)
            {
                return Ok(parkingRecordId);
            }
            else if (reservationId != null)
            {
                return Ok(reservationId);
            }
            return BadRequest("Please provide a userId, parkingRecordId, or reservationId in query");
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
            try
            {
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string tokenUserID = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (tokenUserID == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }
                DetailedPaymentResponseDto payment = paymentServices.GetPayment(paymentId, int.Parse(tokenUserID));
                return Ok(payment);
            }
            catch (PaymentNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }

        }

        // //simple put to make payment
        // [HttpPut("{paymentId}")]
        // public IActionResult Makepayment(int paymentId, [FromBody] MakePaymentRequestDto makePaymentRequestDto)
        // {
        //     try
        //     {
        //         if (httpContextAccessor.HttpContext?.User == null)
        //         {
        //             throw new TokenInvalidException("You are unauthorized");
        //         }
        //         if (!ModelState.IsValid)
        //         {
        //             throw new RequestInvalidException("Invalid Request Model");
        //         }
        //
        //         bool success = paymentServices.MakePayment(paymentId, makePaymentRequestDto);
        //
        //         if (success)
        //         {
        //             return Ok("Payment Success");
        //         }
        //         else
        //         {
        //             return BadRequest("Payment Failed");
        //         }
        //     }
        //     catch (RequestInvalidException ex)
        //     {
        //         return BadRequest(ex);
        //     }
        //     catch (TokenInvalidException ex)
        //     {
        //         return Unauthorized(ex);
        //     }
        //     catch (PaymentNotFoundException ex)
        //     {
        //         return NotFound(ex);
        //     }
        //
        // }

    }
}