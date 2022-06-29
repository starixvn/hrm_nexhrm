namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class BioTasksController : ControllerBase
    {
        private readonly IBotBioTasksService bioTasksService;

        public BioTasksController(IBotBioTasksService bioTasksService)
        {
            this.bioTasksService = bioTasksService;
        }

        [HttpGet]
        public async Task<IActionResult> BioDevice()
        {
            var result = await bioTasksService.SendMessageBioDeviceAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> BioTimeChange()
        {
            var result = await bioTasksService.SendMessageBioTimeChangeAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> BioFingerprintAndCard()
        {
            var result = await bioTasksService.SendMessageBioFingerprintAndCardAynsc().ConfigureAwait(false);
            return Ok(result);
        }
    }
}