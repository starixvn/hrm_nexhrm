namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScheduledMessageController : ControllerBase
    {
        private readonly IScheduledMessageService scheduledMessageService;

        public ScheduledMessageController(IScheduledMessageService scheduledMessageService)
        {
            this.scheduledMessageService = scheduledMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendScheduledMessages()
        {
            var result = await scheduledMessageService.SendScheduledMessages().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendScheduledVegetarianMessages()
        {
            var result = await scheduledMessageService.SendScheduledVegetarianMessages().ConfigureAwait(false);
            return Ok(result);
        }
    }
}