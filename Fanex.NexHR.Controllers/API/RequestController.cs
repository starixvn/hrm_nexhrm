namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;

    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService requestService;
        private readonly INexbotService nexbotService;

        public RequestController(IRequestService requestService, INexbotService nexbotService)
        {
            this.requestService = requestService;
            this.nexbotService = nexbotService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRequest(long requestId, string command, int processedBy)
        {
            LogFileHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = requestId, Command = command, ProcessedBy = processedBy }));
            CommandResult result = await requestService.Process(command, requestId, processedBy);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessMultipleRequest(string requestIds, string command, int processedBy)
        {
            var requests = requestIds.Split(',').Select(long.Parse);
            foreach (var requestId in requests)
            {
                await requestService.Process(command, requestId, processedBy);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> RemindApproveRequest()
        {
            DateTime now = DateTime.Now.Date;
            var pendingRequest = await requestService.GetPendingRequestGroupByApproverAsync();
            await nexbotService.SendMultipleRequestsToApprover(pendingRequest.FirstOrDefault(r => r.ApproverNickname == "Viet"));

            //foreach (var request in pendingRequest)
            //{
            //    if (request.Requests.Any(r => (now - r.CreatedDate.Date).TotalDays >= 3))
            //    {
            //        await nexbotService.SendMultipleRequestsToApprover(request);
            //    }
            //}
            return Ok();
        }

        public IActionResult GetData()
        {
            return Ok("test");
        }
    }
}