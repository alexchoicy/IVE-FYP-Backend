using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [Authorize(Policy = "access-token")]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/parkingrecords")]
    public class ParkingRecordsController : Controller

    {
        public ParkingRecordsController()
        {
        }

        [HttpGet]
        public IActionResult GetParkingRecords()
        {
            return Ok();
        }

        [HttpGet("{parkingRecordId}")]
        public IActionResult GetParkingRecord(int parkingRecordId)
        {
            return Ok(parkingRecordId);
        }

    }
}