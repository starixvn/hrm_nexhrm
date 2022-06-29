namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class EmployeeOnlineController : ControllerBase
    {
        private readonly IEmployeeOnlineService employeeOnlineService;

        public EmployeeOnlineController(IEmployeeOnlineService employeeOnlineService)
        {
            this.employeeOnlineService = employeeOnlineService;
        }

        [HttpGet]
        public IActionResult CheckOnline()
        {
            employeeOnlineService.RunCheckOnline();
            return Ok();
        }
    } 
}