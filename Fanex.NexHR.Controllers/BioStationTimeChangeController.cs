namespace Fanex.NexHR.Controllers
{
    using System;
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
                To = DateTime.Now
            };

            return View(viewModel);
        }

        public ActionResult GetBioStationTimeChange(BioStationTimeChangeViewModel viewModel)
        {
            var criteria = new GetBioTimeChangeCriteria
            {
                From = viewModel.From,
                To = viewModel.To
            };

            var timeChanges = bioService.GetBioTimeChanges(criteria);

            return PartialView("_ListBioTimeChanges", timeChanges);
        }
    }
}
