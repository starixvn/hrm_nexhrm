namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ForgotLogtimeReminderController : ControllerBase
    {
        private readonly IBotFogotLogtimeReminderService fogotLogtimeService;

        public ForgotLogtimeReminderController(IBotFogotLogtimeReminderService fogotLogtimeService)
        {
            this.fogotLogtimeService = fogotLogtimeService;
        }

        [HttpPost]
        public async Task<IActionResult> ForgotLogtime()
        {
            var result = await fogotLogtimeService.SendMessageForgotLogtimeAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}