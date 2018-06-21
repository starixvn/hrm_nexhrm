namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Bio;
    using Fanex.NexHR.Models.Bio;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Bio;

    [Authorize(Roles = "HR")]
    public class BioStationManagementController : BaseController
    {
        private readonly BaseService baseService;

        public BioStationManagementController()
        {
            baseService = new BaseService();
        }

        public ActionResult Index()
        {
            var bioDevices = baseService.GetData<BioDevice>(new GetBioDevicesCriteria());

            var viewModel = bioDevices.Select(x => new BioDeviceViewModel
            {
                DeviceId = x.DeviceId,
                DeviceName = x.DeviceName,
                DisplayId = x.DisplayId,
                DisplayName = x.DisplayName,
                StatusId = x.StatusId,
                Status = x.Status,
                Active = x.Active
            });

            return View(viewModel);
        }
    }
}