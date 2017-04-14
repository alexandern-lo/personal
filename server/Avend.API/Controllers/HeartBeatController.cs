using System;
using System.Collections.Generic;

using Avend.API.Infrastructure.Responses;
using Avend.API.Model;
using Avend.API.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.SwaggerGen.Annotations;

namespace Avend.API.Controllers
{
    [Route("/")]
    public class HeartBeatController : BaseController
    {
        public HeartBeatController(DbContextOptions<AvendDbContext> options) :
            base(options)
        {
        }

        [HttpOptions]
        public IActionResult GetOptions()
        {
            return Ok();
        }

        /// <summary>
        /// Root endpoint to indicate heartbeat and prevent error messages from crawlers.
        /// </summary>
        /// 
        /// <returns>HTTP ActionResult object with proper HTTP code and response body</returns>
        /// 
        /// <remarks>Returns server local and utc time for heartbeat indication</remarks>
        /// 
        /// <response code="200"></response>
        [HttpGet("/")]
        [SwaggerOperation("GetHeartBeat")]
        [ProducesResponseType(typeof(OkResponse<Dictionary<string, object>>), 200)]
        public OkObjectResult GetHeartBeat()
        {
            return Ok(new OkResponse<Dictionary<string, object>>()
            {
                Data = new Dictionary<string, object>()
                    {
                        {"server_time_utc", DateTime.UtcNow.ToString("O")},
                        {"server_time_local", DateTime.Now.ToString("O")},
                    }
            });
        }


        /// <summary>
        /// Root endpoint to indicate heartbeat and prevent error messages from crawlers
        /// for API v1.
        /// </summary>
        /// 
        /// <returns>HTTP ActionResult object with proper HTTP code and response body</returns>
        /// 
        /// <remarks>Returns api version as well as server local and utc time for heartbeat indication</remarks>
        /// 
        /// <response code="200"></response>
        [HttpGet("/api/v1")]
        [SwaggerOperation("GetHeartBeatV1")]
        [ProducesResponseType(typeof(OkResponse<Dictionary<string, object>>), 200)]
        public OkObjectResult GetHeartBeatV1()
        {
            return Ok(new OkResponse<Dictionary<string, object>>()
            {
                Data = new Dictionary<string, object>()
                    {
                        {"api_version", 1},
                        {"server_time_utc", DateTime.UtcNow.ToString("O")},
                        {"server_time_local", DateTime.Now.ToString("O")},
                    }
            });
        }
    }
}
