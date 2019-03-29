namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System.Threading.Tasks;
    using System.Web.Http;

    public class ScheduledMessageController : ApiController
    {
        private readonly IScheduledMessageService scheduledMessageService;

        public ScheduledMessageController()
        {
            scheduledMessageService = new ScheduledMessageService();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> SendScheduledMessage()
        {
            var result = await scheduledMessageService.SendScheduledMessages().ConfigureAwait(false);
            return Json(result);
        }
    }
}