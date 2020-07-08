namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Interfaces;

    public class LateInEarlyOutViolateOnMonthWarningController : ApiController
    {
        private readonly IBotAutoJobService _botService;

        public LateInEarlyOutViolateOnMonthWarningController() : this(null)
        {
        }

        public LateInEarlyOutViolateOnMonthWarningController(
            IBotAutoJobService botService
        )
        {
            _botService = botService ?? new BotLateInEarlyOutViolateOnMonthWarningSerivce();
        }

        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> LateInEarlyOutViolateOnMonth()
        {
            var result = await _botService.SendMessageAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}