namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScheduledVegetarianMessagesController : ControllerBase
    {
        private readonly IScheduledMessageService scheduledMessageService;

        public ScheduledVegetarianMessagesController(IScheduledMessageService scheduledMessageService)
        {
            this.scheduledMessageService = scheduledMessageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendScheduledVegetarianMessages()
        {
            var result = await scheduledMessageService.SendScheduledVegetarianMessages().ConfigureAwait(false);
            return Ok(result);
        }
    }
}
