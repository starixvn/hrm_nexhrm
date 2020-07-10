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

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> SendScheduledMessages()
        {
            var result = await scheduledMessageService.SendScheduledMessages().ConfigureAwait(false);
            return Ok(result);
        }
    }
}