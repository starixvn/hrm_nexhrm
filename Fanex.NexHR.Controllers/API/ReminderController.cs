namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.Email;

    public class ReminderController : ApiController
    {
        private readonly MailBirthdayAndFirstWorkingdayReminderService mailReminderService;

        public ReminderController()
        {
            mailReminderService = new MailBirthdayAndFirstWorkingdayReminderService();
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> BirthdayAndFirstWorkingday()
        {
            var result = await mailReminderService.ReminderBirthdayAndFirstWorkingday().ConfigureAwait(false);
            return Ok(result);
        }
    }
}