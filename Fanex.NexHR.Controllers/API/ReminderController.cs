namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.Email;

    public class ReminderController : ApiController
    {
        private readonly MailReminderService mailReminderService;

        public ReminderController()
        {
            mailReminderService = new MailReminderService();
        }

        [HttpPost]
        public async Task BirthdayAndFirstWorkingday()
        {
            await mailReminderService.ReminderBirthdayAndFirstWorkingday().ConfigureAwait(false);
        }
    }
}