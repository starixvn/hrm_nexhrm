namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReminderController : ControllerBase
    {
        private readonly IMailBirthdayAndFirstWorkingdayReminderService mailReminderService;
        private readonly IBotAutoJobService botService;

        public ReminderController(
            IMailBirthdayAndFirstWorkingdayReminderService mailReminderService,
            IBotAutoJobService botService
        )
        {
            this.mailReminderService = mailReminderService;
            this.botService = botService;
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
    }
}