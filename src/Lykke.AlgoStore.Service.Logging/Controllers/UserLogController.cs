using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Requests;
using Lykke.AlgoStore.Service.Logging.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Service.Logging.Controllers
{
    [Authorize]
    [Route("api/v1/userLog")]
    public class UserLogController : Controller
    {
        private readonly IUserLogService _service;

        public UserLogController(IUserLogService service)
        {
            _service = service;
        }

        [HttpPost("writeLog")]
        [SwaggerOperation("WriteLog")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task<IActionResult> WriteLog([FromBody] UserLogRequest userLog)
        {
            await _service.WriteAsync(userLog);

            return NoContent();
        }

        [HttpPost("writeMessage")]
        [SwaggerOperation("WriteMessage")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> WriteMessage(string instanceId, string message)
        {
            await _service.WriteAsync(instanceId, message);

            return NoContent();
        }

        [HttpPost("writeLogs")]
        [SwaggerOperation("WriteLogs")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> WriteLogs([FromBody] List<UserLogRequest> userLogs)
        {
            await _service.WriteAsync(userLogs);

            return NoContent();
        }

        [HttpGet("tailLog")]
        [SwaggerOperation("GetTailLog")]
        [ProducesResponseType(typeof(IEnumerable<UserLogResponse>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> GetTailLog(TailLogRequest tailLog)
        {
            var result = await _service.GetTailLog(tailLog.Tail, tailLog.InstanceId);

            return Ok(result);
        }
    }
}
