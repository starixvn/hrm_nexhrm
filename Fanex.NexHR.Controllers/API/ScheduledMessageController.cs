namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;
    using System.Threading.Tasks;
    using System.Web.Http;

    [Authorize]
    [JwtAuthentication]
    public class ScheduledMessageController : ApiController
    {
        private readonly IScheduledMessageService scheduledMessageService;

        public ScheduledMessageController()
        {
            scheduledMessageService = new ScheduledMessageService();
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendScheduledMessages()
        {
            var result = await scheduledMessageService.SendScheduledMessages().ConfigureAwait(false);
            return Ok(result);
        }
    }
}