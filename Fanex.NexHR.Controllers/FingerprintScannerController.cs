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
                Type = viewModel.Type
            };

            var listFingerprintData = _bioService.GetFingerprintAndCardSummary(criteria);

            return PartialView("_ListFingerprint", listFingerprintData);
        }

        public ActionResult Fingerprint(int empId)
        {
            var criteria = new GetFingerprintDetailByEmployeeIdCriteria { EmpId = empId };

            var fingers = _bioService.GetFingerprintDetailByEmployeeId(criteria);

            return View(fingers);
        }

        public ActionResult Card(int empId)
        {
            var criteria = new GetCardDetailByEmployeeIdCriteria { EmpId = empId };

            var fingers = _bioService.GetCardDetailByEmployeeId(criteria);

            return View(fingers);
        }

        [HttpPost]
        public ActionResult ChangeFingerStatus(int employeeId, int bioUserId, int index, FingerprintScannerAction status)
        {
            var criteria = new UpdateFingerprintStatusCriteria
            {
                EmployeeId = employeeId,
                BioUserId = bioUserId,
                FingerIndex = index,
                Status = status,
                UpdatedBy = CurrentUser.Id
            };

            var commandResult = _commandService.Update(criteria);

            return Json(new { commandResult });
        }

        public ActionResult ChangeCardStatus(int employeeId, int cardId, FingerprintScannerAction status)
        {
            var criteria = new UpdateCardStatusCriteria
            {
                EmployeeId = employeeId,
                CardId = cardId,
                Status = status,
                UpdatedBy = CurrentUser.Id
            };

            var commandResult = _commandService.Update(criteria);

            return Json(new { commandResult });
        }
    }
}