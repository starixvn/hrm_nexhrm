namespace Fanex.NexHR.Controllers.API
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services.AutoJobService;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;

    [Authorize]
    [JwtAuthentication]
    public class BioTasksController : ApiController
    {
        private readonly IBotBioTasksService bioTasksService;

        public BioTasksController()
        {
            bioTasksService = new BioTasksService();
        }

        [HttpGet]
        public async Task<IHttpActionResult> BioDevice()
        {
            var result = await bioTasksService.SendMessageBioDeviceAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> BioTimeChange()
        {
            var result = await bioTasksService.SendMessageBioTimeChangeAsync().ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> BioFingerprintAndCard()
        {
            var result = await bioTasksService.SendMessageBioFingerprintAndCardAynsc().ConfigureAwait(false);
            return Ok(result);
        }
    }
}