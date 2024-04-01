using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Exceptions;
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
    [Route("api/v{version:apiVersion}/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationServices reservationServices;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ReservationsController(IReservationServices reservationServices, IHttpContextAccessor httpContextAccessor)
        {
            this.reservationServices = reservationServices;
            this.httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public async Task<IActionResult> GetReservations(int? parkingLotId, int? userId, int? vehicleId)
        {
            try
            {
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string tokenUserId = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (tokenUserId == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }

                if (userId == null || !httpContextAccessor.HttpContext.User.IsInRole("admin"))
                {
                    userId = int.Parse(tokenUserId);
                }

                IEnumerable<ReservationResponseDto> reservations;
               if (parkingLotId != null)
                {
                    reservations = await reservationServices.getReservationsByLotID(parkingLotId.Value);
                }
                else if (userId != null)
                {
                    reservations = await reservationServices.getReservationsByUserID(userId.Value);
                }

                else if (vehicleId != null)
                {
                    reservations = reservationServices.getReservationsByVehicleID(vehicleId.Value);
                }

                else
                {
                    throw new ArgumentException("At least one parameter must be provided");
                }
                return Ok(reservations);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequestDto createReservationRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new RequestInvalidException("Invalid request model");
                }
                if (httpContextAccessor.HttpContext?.User == null)
                {
                    throw new TokenInvalidException("You are unauthorized");
                }
                string tokenUserid = httpContextAccessor.HttpContext.User.getUserID() ?? "";
                if (tokenUserid == "")
                {
                    throw new TokenInvalidException("The Token is invalid");
                }
                bool success = await reservationServices.createReservation(int.Parse(tokenUserid), createReservationRequestDto);
                return Ok(success);
            }
            catch (TokenInvalidException ex)
            {
                return Unauthorized(ex);
            }
            catch (RequestInvalidException ex)
            {
                return BadRequest(ex);
            }
            catch (vehicleNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (ParkingLotNotFoundException ex)
            {
                return NotFound(ex);
            }
            catch (InvalidReservationTimeException ex)
            {
                return BadRequest(ex);
            }
            catch (InvalidSpaceTypeException ex)
            {
                return BadRequest(ex);
            }
            catch (NoAvailableSpacesException ex)
            {
                return BadRequest(ex);
            }
            catch (ReservationLimitExceededException ex)
            {
                return BadRequest(ex);
            }
            catch (ReservationTimeConflictException ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{reservationId}")]
        public IActionResult GetReservation(int reservationId)
        {
            try
            {
                ReservationResponseDto reservation = reservationServices.getReservationByID(reservationId);
                return Ok(reservation);
            }
            catch (ReservationNotFoundException ex)
            {
                return NotFound(ex);
            }
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

        [HttpPost("{reservationId}/payment")]
        public IActionResult MakePayment(int reservationId, [FromBody] MakePaymentRequestDto makePaymentRequestDto)
        {
            bool tryParse = Enum.TryParse(makePaymentRequestDto.paymentMethodType, out PaymentMethodType paymentMethodType);
            if (!tryParse)
            {
                return BadRequest("Invalid payment method");
            }

            string result = reservationServices.MakeReservationPayment(reservationId, paymentMethodType);
            return Ok(result);
        }

    }
}