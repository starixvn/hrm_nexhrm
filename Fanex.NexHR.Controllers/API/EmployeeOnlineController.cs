namespace Fanex.NexHR.Controllers.API
{
    using System.Web.Http;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Interfaces;

    //[Authorize]
    //[JwtAuthentication]
    public class EmployeeOnlineController : ApiController
    {
        private readonly IEmployeeOnlineService employeeOnlineService;

        public EmployeeOnlineController()
            : this(null)
        {
        }

        public EmployeeOnlineController(IEmployeeOnlineService employeeOnlineService)
        {
            this.employeeOnlineService = employeeOnlineService ?? new EmployeeOnlineService();
        }

        [HttpGet]
        public IHttpActionResult CheckOnline()
        {
            employeeOnlineService.RunCheckOnline();
            return Ok();
        }
    } 
}