namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Criteria.Employee;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;

    [Authorize]
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
        public async Task<IActionResult> ProcessRequest(long requestId, string command, string nickName)
        {
            LogFileHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = requestId, Command = command, Nickname = nickName }));
            var criteria = new GetEmployeeByNickNameCriteria
            {
                NickName = nickName
            };

            var approver = employeeService.GetEmployeeByNickName(criteria);

            CommandResult result = await requestService.Process(command, requestId, approver.Id);
            return Ok(result);
        }

        public IActionResult GetData()
        {
            return Ok("test");
        }
    }
}