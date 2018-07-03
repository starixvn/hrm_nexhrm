namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Core.Enums;
    using Criteria.Bio;
    using Services;
    using Services.Interfaces;
    using ViewModels;

    [Authorize(Roles = "HR")]
    public class FingerprintScannerController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IBioService _bioService;
        private readonly ICommandService _commandService;

        public FingerprintScannerController()
            : this(null, null, null)
        {
        }

        public FingerprintScannerController(
            ITeamService teamService,
            IBioService bioService,
            ICommandService commandService)
        {
            _teamService = teamService ?? new TeamService();
            _bioService = bioService ?? new BioService();
            _commandService = commandService ?? new CommandService();
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
                Type = viewModel.Type,
                Status = viewModel.Status
            };

            var listFingerprintData = _bioService.GetFingerprintAndCardSummary(criteria);

            return PartialView("_ListFingerprint", listFingerprintData);
        }

        [HttpPost]
        public ActionResult ChangeStatus(int employeeId, int bioUserId, FingerprintScannerStatus status)
        {
            var criteria = new UpdateFingerprintAndCardStatusCriteria
            {
                EmployeeId = employeeId,
                BioUserId = bioUserId,
                Status = status,
                UpdatedBy = CurrentUser.Id
            };

            var commandResult = _commandService.Update(criteria);

            return Json(new { commandResult });
        }
    }
}