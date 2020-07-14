namespace Fanex.NexHR.Controllers.API
{
    using Criteria.Employee;
    using Fanex.NexHR.Web.API;
    using Models;
    using Services;
    using Services.Interfaces;
    using System.Web.Http;

    [Authorize]
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
            var criteria = new GetEmployeeByNickNameCriteria
            {
                 NickName = nickName
            };

            var approver = employeeService.GetEmployeeByNickName(criteria);

            CommandResult result = requestService.Process(command, requestId, approver.Id);
            return Json(result);
        }

        [JwtAuthentication]
        public IHttpActionResult GetData()
        {
            return Json("test");
        }
    }
}