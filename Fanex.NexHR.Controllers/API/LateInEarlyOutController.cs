namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LateInEarlyOutController : ControllerBase
    {
        private readonly IBotLateInLateOutAutoJobService botService;

        public LateInEarlyOutController(
            IBotLateInLateOutAutoJobService botService
        )
        {
            this.botService = botService;
        }

        [HttpPost]
        public async Task<IActionResult> LateInEarlyOutViolateOnMonth()
        {
            var result = await botService.SendMessageViolateOnMonthAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ViolateDiscipline()
        {
            var result = await botService.SendMessageViolateDisciplineAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Deduct()
        {
            var result = await botService.SendMessageDeductAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AutoDeduct()
        {
            var result = await botService.DeductAsync().ConfigureAwait(false);
            return Ok(result);
        }
    }
}