namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Criteria.Bio;
    using Services;
    using Services.Interfaces;
    using ViewModels.Bio;

    [Authorize(Roles = "HR")]
    public class BioStationTimeChangeController : BaseController
    {
        private readonly IBioService bioService;

        public BioStationTimeChangeController() : this(null)
        {
        }

        public BioStationTimeChangeController(IBioService bioService)
        {
            this.bioService = bioService ?? new BioService();
        }

        public ActionResult Index()
        {
            var viewModel = new BioStationTimeChangeViewModel
            {
                TimeChanges = bioService.GetBioTimeChanges(new GetBioTimeChangeCriteria())
            };

            return View(viewModel);
        }
    }
}
