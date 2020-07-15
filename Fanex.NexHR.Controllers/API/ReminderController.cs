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
        private readonly MailBirthdayAndFirstWorkingdayReminderService _mailReminderService;
        private readonly IBotAutoJobService _botService;

        public ReminderController() : this(null, null)
        {
        }

        public ReminderController(
            MailBirthdayAndFirstWorkingdayReminderService mailReminderService,
            IBotAutoJobService botService
        )
        {
            _mailReminderService = mailReminderService ?? new MailBirthdayAndFirstWorkingdayReminderService();
            _botService = botService ?? new BotMissingCheckInCheckOutReminderSerivce();
        }

        [HttpPost]
        public async Task<IHttpActionResult> BirthdayAndFirstWorkingday()
        {
            var result = await _mailReminderService.ReminderBirthdayAndFirstWorkingday().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> MissingCheckInCheckOut()
        {
            var result = await _botService.SendMessageAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}