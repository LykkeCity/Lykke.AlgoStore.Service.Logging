using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Logging.Core.Services;
using Lykke.AlgoStore.Service.Logging.Requests;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Service.Logging.Controllers
{
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
    }
}
