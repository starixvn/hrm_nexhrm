namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Email;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;

    [Authorize]
    [JwtAuthentication]
    public class ReminderController : ApiController
    {
        private readonly MailBirthdayAndFirstWorkingdayReminderService mailReminderService;
        private readonly IBotAutoJobService botService;

        public ReminderController() : this(null, null)
        {
        }

        public ReminderController(
            MailBirthdayAndFirstWorkingdayReminderService mailReminderService,
            IBotAutoJobService botService
        )
        {
            this.mailReminderService = mailReminderService ?? new MailBirthdayAndFirstWorkingdayReminderService();
            this.botService = botService ?? new BotMissingCheckInCheckOutReminderSerivce();
        }

        [HttpPost]
        public async Task<IHttpActionResult> BirthdayAndFirstWorkingday()
        {
            var result = await mailReminderService.ReminderBirthdayAndFirstWorkingday().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> MissingCheckInCheckOut()
        {
            var result = await botService.SendMessageAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}