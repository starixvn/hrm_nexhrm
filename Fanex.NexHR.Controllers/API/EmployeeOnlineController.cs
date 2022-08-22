namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
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
        public async Task<IActionResult> CheckOnline()
        {
            await employeeOnlineService.RunCheckOnline();
            return Ok();
        }
    }
}