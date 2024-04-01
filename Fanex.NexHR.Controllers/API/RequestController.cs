namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;

   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService requestService;

        public RequestController(IRequestService requestService)
        {
            this.requestService = requestService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRequest(long requestId, string command, int processedBy)
        {
            LogFileHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = requestId, Command = command, ProcessedBy = processedBy }));
            CommandResult result = await requestService.Process(command, requestId, processedBy);
            return Ok(result);
        }

        public IActionResult GetData()
        {
            return Ok("test");
        }
    }
}