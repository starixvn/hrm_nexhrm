namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimeSheetService timeSheetService;
        private readonly ILogServiceProvider logServiceProvider;

        public TimesheetController(ITimeSheetService timeSheetService,
            ILogServiceProvider logServiceProvider)
        {
            this.timeSheetService = timeSheetService;
            this.logServiceProvider = logServiceProvider;
        }

        [HttpGet]
        public async Task<IActionResult> ReminderReachedWorkingTime()
        {
            logServiceProvider.LogFile("Call API ReminderReachedWorkingTime");

            await timeSheetService.SendReminderReachedWorkingTime();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ImportDataFromBioStar()
        {
            await timeSheetService.ImportDataFromBioStar();
            return Ok();
        }
    }
}