namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.CheckInCheckOut;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models.CheckInCheckOut;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels.CheckInOutRequest;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService requestService;
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly ILogServiceProvider logService;

        public RequestController(IRequestService requestService,
            INexbotService nexbotService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            ILogServiceProvider logService)
        {
            this.requestService = requestService;
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.environment = environment;
            this.logService = logService;
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
            var results = new List<ProcessRequestResult>();
            foreach (var requestId in requests)
            {
                var result = await requestService.Process(command, requestId, processedBy);
                results.Add(new ProcessRequestResult
                {
                    RequestId = requestId,
                    Code = result.Code,
                    Message = result.Message
                });
            }
            return Ok(results);
        }

        [HttpGet]
        public async Task<IActionResult> RemindApproveRequest()
        {
            var now = DateTimeOffset.Now.Date;
            var pendingRequest = await requestService.GetPendingRequestGroupByApproverAsync();

            foreach (var request in pendingRequest)
            {
                if (request.Requests.Any(r => (now - r.CreatedAt.Date).TotalDays >= 3))
                {
                    await nexbotService.SendMultipleRequestsToApprover(request);
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GetCheckInCheckOutForm(string conversationId)
        {
            var criteria = new GetCheckInCheckOutByConversationIdCriteria { ConversationId = conversationId };
            var model = await baseService.GetDataEntityAsync<CheckInCheckOutAdaptiveCardForm>(criteria);
            var time = DateTime.ParseExact(model.MissingType == 2 ? model.EndCheckOutTime : model.BeginCheckInTime, "HH:mm", System.Globalization.CultureInfo.CurrentCulture);
            var attendance = model.AttendanceDate.AddHours(time.Hour);
            model.AttendanceDate = attendance;
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFormCheckInCheckOut(CheckInOutRequestViewModel model)
        {
            var criteria = new InsertRequestCriteria
            {
                EmpId = model.EmployeeId,
                RequestType = model.CheckInOutType,
                IsPlanned = false,
                IsCrossDate = model.IsCrossDate,
                CreatedDate = DateTimeOffset.Now.DateTime,
                CreatedBy = model.EmployeeId,
                FromDate = model.WorkDate
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                SendEmailAndChatBot(model);
            }

            return Ok(result);
        }

        private void SendEmailAndChatBot(CheckInOutRequestViewModel model)
        {
            model.EmailTemplatePath = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForCheckInOut);
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var leaders = await requestService.GetLeaderForCheckInOut(model.EmployeeId, model.IsStandbyAtWeekend);
                    if (leaders != null && leaders.Any())
                    {
                        requestService.SendMailForCheckInOut(model, leaders);
                        await nexbotService.SendMessageForCheckInOutRequest(model, leaders);
                    }
                }
                catch (Exception ex)
                {
                    logService.LogException(ex);
                }
            });
        }
    }
}