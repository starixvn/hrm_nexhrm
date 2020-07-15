namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;

    [Authorize]
    [JwtAuthentication]
    public class LateInEarlyOutController : ApiController
    {
        private readonly IBotLateInLateOutAutoJobService _botService;

        public LateInEarlyOutController() : this(null)
        {
        }

        public LateInEarlyOutController(
            IBotLateInLateOutAutoJobService botService
        )
        {
            _botService = botService ?? new BotLateInEarlyOutViolateOnMonthWarningSerivce();
        }

        [HttpPost]
        public async Task<IHttpActionResult> LateInEarlyOutViolateOnMonth()
        {
            var result = await _botService.SendMessageViolateOnMonthAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ViolateDiscipline()
        {
            var result = await _botService.SendMessageViolateDisciplineAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Deduct()
        {
            var result = await _botService.SendMessageDeductAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}