namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Criteria.Employee;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Web.API;
    using Models;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;

    [Authorize]
    [JwtAuthentication]
    public class RequestController : ApiController
    {
        private readonly IRequestService requestService;
        private readonly IEmployeeService employeeService;

        public RequestController()
            : this(null, null)
        {
        }

        public RequestController(
            IRequestService requestService,
            IEmployeeService employeeService)
        {
            this.requestService = requestService ?? new RequestService();
            this.employeeService = employeeService ?? new EmployeeService();
        }

        [HttpPost]
        public IHttpActionResult ProcessRequest(long requestId, string command, string nickName)
        {
            LogFileHelper.WriteInfo(JsonConvert.SerializeObject(new { RequestId = requestId, Command = command, Nickname = nickName }));
            var criteria = new GetEmployeeByNickNameCriteria
            {
                NickName = nickName
            };

            var approver = employeeService.GetEmployeeByNickName(criteria);

            CommandResult result = requestService.Process(command, requestId, approver.Id);
            return Json(result);
        }

        public IHttpActionResult GetData()
        {
            return Json("test");
        }
    }
}