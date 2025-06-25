namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TimesheetController(
        ITimeSheetService timeSheetService,
        IBioWebApiService bioWebApiService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> ReminderReachedWorkingTime()
        {
            await timeSheetService.SendReminderReachedWorkingTime();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ImportDataFromBioStar()
        {
            await bioWebApiService.ImportTimeRecordAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> NotifyEmployeeAfterCheckOut()
        {
            await timeSheetService.NotifyEmployeeAfterCheckOut();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> NotifyEmployeeAfterCheckIn()
        {
            await timeSheetService.NotifyEmployeeAfterCheckIn();
            return Ok();
        }
    }
}