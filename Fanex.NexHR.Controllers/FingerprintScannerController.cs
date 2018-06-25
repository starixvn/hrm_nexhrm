namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Criteria.Bio;
    using Services;
    using Services.Interfaces;
    using ViewModels;

    public class FingerprintScannerController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IBioService _bioService;

        public FingerprintScannerController()
            : this(null, null)
        {
        }

        public FingerprintScannerController(
            ITeamService teamService,
            IBioService bioService)
        {
            _teamService = teamService ?? new TeamService();
            _bioService = bioService ?? new BioService();
        }

        public ActionResult Index()
        {
            var viewModel = new FingerprintAndCardViewModel
            {
                Teams = _teamService.GetAll()
            };
            return View(viewModel);
        }

        public ActionResult GetFingerprintScannerData(FingerprintAndCardViewModel viewModel)
        {
            var criteria = new GetFingerprintAndCardSummaryCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                GroupId = viewModel.TeamId,
                Type = viewModel.Type
            };

            var listFingerprintData = _bioService.GetFingerprintAndCardSummary(criteria);

            return PartialView("_ListFingerprint", listFingerprintData);
        }
    }
}