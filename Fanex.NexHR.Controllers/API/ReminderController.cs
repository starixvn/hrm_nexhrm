namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReminderController : ControllerBase
    {
        private readonly IMailBirthdayAndFirstWorkingdayReminderService mailReminderService;
        private readonly IBotAutoJobService botService;
        private readonly IBotLateInLateOutAutoJobService botLateInLateOut;
        private readonly IBotLogTimeRemindService botLogTime;

        public ReminderController(
            IMailBirthdayAndFirstWorkingdayReminderService mailReminderService,
            IBotLateInLateOutAutoJobService botLateInLateOut,
            IBotAutoJobService botService,
            IBotLogTimeRemindService botLogTime
        )
        {
            this.mailReminderService = mailReminderService;
            this.botLateInLateOut = botLateInLateOut;
            this.botService = botService;
            this.botLogTime = botLogTime;
        }

        [HttpPost]
        public async Task<IActionResult> BirthdayAndFirstWorkingday()
        {
            var result = await mailReminderService.ReminderBirthdayAndFirstWorkingday().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> MissingCheckInCheckOut()
        {
            var result = await botService.SendMessageAsync().ConfigureAwait(false);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> LateInEarlyOutMoreThanSixtyMinute()
        {
            var result = await botLateInLateOut.ReminderLateInEarlyOutMoreThanSixtyMinute().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MissingLogTime()
        {
            var result = await botLogTime.SendMessageLogTimeRemindAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}