namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimeSheetService timeSheetService;

        public TimesheetController(ITimeSheetService timeSheetService)
        {
            this.timeSheetService = timeSheetService;
        }

        [HttpGet]
        public async Task<IActionResult> ReminderReachedWorkingTime()
        {
            LogFileHelper.WriteInfo("Call API ReminderReachedWorkingTime");

            await timeSheetService.SendReminderReachedWorkingTime();
            return Ok();
        }
    }
}