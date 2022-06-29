namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Email;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ReminderController : ControllerBase
    {
        private readonly MailBirthdayAndFirstWorkingdayReminderService mailReminderService;
        private readonly IBotAutoJobService botService;

        public ReminderController(
            MailBirthdayAndFirstWorkingdayReminderService mailReminderService,
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