namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Criteria.Employee;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService requestService;
        private readonly IEmployeeService employeeService;

        public RequestController(
            IRequestService requestService,
            IEmployeeService employeeService)
        {
            this.requestService = requestService;
            this.employeeService = employeeService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessRequest(long requestId, string command, string nickName, int processedBy)
        {
            LogFileHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = requestId, Command = command, Nickname = nickName, ProcessedBy = processedBy }));

            if (!string.IsNullOrWhiteSpace(nickName))
            {
                var criteria = new GetEmployeeByNickNameCriteria
                {
                    NickName = nickName
                };

                var approver = await employeeService.GetEmployeeByNickName(criteria);
                processedBy = approver.Id;
            }

            CommandResult result = await requestService.Process(command, requestId, processedBy);
            return Ok(result);
        }

        public IActionResult GetData()
        {
            return Ok("test");
        }
    }
}