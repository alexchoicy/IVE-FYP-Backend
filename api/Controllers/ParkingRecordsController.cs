using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models.Request;
using api.Models.Respone;
using api.Services;
using api.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/users/{userid}/parkingrecords/")]
    [Authorize]
    public class ParkingRecordsController : ControllerBase

    {
        private readonly IParkingRecordServices parkingRecordServices;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPaymentServices paymentServices;

        public ParkingRecordsController(IParkingRecordServices parkingRecordServices,
            IHttpContextAccessor httpContextAccessor, IPaymentServices paymentServices)
        {
            this.parkingRecordServices = parkingRecordServices;
            this.httpContextAccessor = httpContextAccessor;
            this.paymentServices = paymentServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingRecords(int userid, int? recordsPerPage, int? page)
        {
            recordsPerPage = recordsPerPage ?? 15;
            page = page ?? 1;

            PagedResponse<ICollection<ParkingRecordResponseDtoDetailed>> records =
                await parkingRecordServices.GetParkingRecordsAsync(userid, recordsPerPage.Value, page.Value);
            Console.WriteLine(JsonConvert.SerializeObject(records));
            return Ok(records);
        }

        [HttpGet("{sessionID}")]
        public async Task<IActionResult> GetParkingRecord(int userid, int sessionID)
        {
            ParkingRecordResponseDtoDetailed record = await parkingRecordServices.GetParkingRecord(userid, sessionID);

            return Ok(record);
        }

        [HttpGet("{sessionID}/payment")]
        public IActionResult GetPayment(int userid, int sessionID)
        {
            decimal payment = paymentServices.GetTotalPrices(userid, sessionID);

            return Ok(payment);
        }

        [HttpPost("{sessionID}/payment")]
        public IActionResult MakePayment(int userid, int sessionID, [FromBody] MakePaymentRequestDto paymentMethod)
        {
            Console.WriteLine(JsonConvert.SerializeObject(paymentMethod));
            bool tryParse = Enum.TryParse(paymentMethod.paymentMethodType, out PaymentMethodType paymentMethodType);
            if (!tryParse)
            {
                return BadRequest("Invalid payment method");
            }

            string payment = paymentServices.MakeParkingPaymentAsync(userid, sessionID, paymentMethodType);

            return Ok(payment);
        }
    }
}