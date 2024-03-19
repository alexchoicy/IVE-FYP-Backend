using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Respone;
using api.Services;
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

        public ParkingRecordsController(IParkingRecordServices parkingRecordServices, IHttpContextAccessor httpContextAccessor)
        {
            this.parkingRecordServices = parkingRecordServices;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingRecords(int userid, int? recordsPerPage, int? page)
        {
            recordsPerPage = recordsPerPage ?? 15;
            page = page ?? 1;

            IEnumerable<ParkingRecordResponseDtoDetailed> records = await parkingRecordServices.GetParkingRecordsAsync(userid, recordsPerPage.Value, page.Value);
            Console.WriteLine(JsonConvert.SerializeObject(records));
            return Ok(records);
        }

        [HttpGet("{sessionID}")]
        public async Task<IActionResult> GetParkingRecord(int userid, int sessionID)
        {
            ParkingRecordResponseDtoDetailed record = await parkingRecordServices.GetParkingRecord(userid, sessionID);

            return Ok(record);
        }

    }
}