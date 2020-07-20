namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Web.API;
    using System.Web.Http;
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;

    [Authorize]
    [JwtAuthentication]
    public class ForgotLogtimeReminderController : ApiController
    {
        private readonly IBotFogotLogtimeReminderService fogotLogtimeService;

        public ForgotLogtimeReminderController()
        {
            fogotLogtimeService = new FogotLogtimeReminderService();
        }

        [HttpPost]
        public async Task<IHttpActionResult> ForgotLogtime()
        {
            var result = await fogotLogtimeService.SendMessageForgotLogtimeAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
