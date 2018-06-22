namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Models.FingerprintScanner;
    using Services;
    using Services.Interfaces;
    using ViewModels;

    public class FingerprintScannerController : BaseController
    {
        private readonly ITeamService _teamService;

        public FingerprintScannerController()
            : this(null)
        {
        }

        public FingerprintScannerController(
            ITeamService teamService)
        {
            _teamService = teamService ?? new TeamService();
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
            var listFingerprintData = new List<FingerprintAndCard>();
            return PartialView("_ListFingerprint", listFingerprintData);
        }
    }
}