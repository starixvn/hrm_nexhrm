namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Bio;
    using Fanex.NexHR.Models.Bio;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Bio;

    [Authorize(Roles = "HR")]
    public class BioStationManagementController : BaseController
    {
        private readonly BaseService baseService;
        private readonly ICommandService commandService;

        public BioStationManagementController()
        {
            baseService = new BaseService();
            commandService = new CommandService();
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

        public ActionResult EditBioDevice(int id)
        {
            var bioDevice = baseService.GetDataEntity<BioDevice>(new GetBioDeviceCriteria { DeviceId = id });

            var viewModel = new EditBioDeviceViewModel
            {
                DeviceId = bioDevice.DeviceId,
                DisplayId = bioDevice.DisplayId,
                StatusId = bioDevice.StatusId,
                BioDeviceStatuses = baseService.GetData<BioDeviceStatus>(new GetBioDeviceStatusesCriteria())
                .Select(x => new SelectListItem
                {
                    Value = x.StatusId.ToString(),
                    Text = x.Name
                }),
                BioDeviceDisplayNames = baseService.GetData<BioDeviceDisplayName>(new GetBioDeviceDisplayNamesCriteria())
                .Select(x => new SelectListItem
                {
                    Value = x.DisplayId.ToString(),
                    Text = x.Name
                })
            };

            return PartialView("_EditBioDevice", viewModel);
        }

        [HttpPost]
        public ActionResult EditBioDevice(EditBioDeviceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new EditBioDeviceCriteria
                {
                    DeviceId = viewModel.DeviceId,
                    DisplayId = viewModel.DisplayId,
                    StatusId = viewModel.StatusId,
                    UpdatedBy = CurrentUser.Id
                };

                var result = commandService.Update(criteria);

                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    message = result.Message
                });
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);

            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        public ActionResult ReloadBioDevice(int id)
        {
            var bioDevice = baseService.GetDataEntity<BioDevice>(new GetBioDeviceCriteria { DeviceId = id });

            var viewModel = new BioDeviceViewModel
            {
                DeviceId = bioDevice.DeviceId,
                DeviceName = bioDevice.DeviceName,
                DisplayId = bioDevice.DisplayId,
                DisplayName = bioDevice.DisplayName,
                StatusId = bioDevice.StatusId,
                Status = bioDevice.Status,
                Active = bioDevice.Active
            };

            return PartialView("_BioDeviceRow", viewModel);
        }
    }
}