namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enums;
    using Criteria.Bio;
    using Criteria.LookupData;
    using Models;
    using Models.Bio;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Bio;

    [Authorize(Roles = "HR")]
    public class BioController : BaseController
    {
        private readonly BaseService baseService;
        private readonly ICommandService commandService;
        private readonly IBioService bioService;
        private readonly ITeamService teamService;

        public BioController()
        {
            baseService = new BaseService();
            commandService = new CommandService();
            bioService = new BioService();
            teamService = new TeamService();
        }

        public ActionResult BioDevice()
        {
            var bioDevices = baseService.GetData<BioDevice>(new GetBioDevicesCriteria());

            var viewModel = bioDevices.Select(x => new BioDeviceViewModel
            {
                DeviceId = x.DeviceId,
                DeviceName = x.DeviceName,
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
                DisplayName = bioDevice.DisplayName,
                StatusId = bioDevice.StatusId,
                BioDeviceStatuses = baseService.GetData<LookupData>(new GetAllLookupDataCriteria { LookupCode = LookupCodeName.BioDeviceStatus })
                .Select(x => new SelectListItem
                {
                    Value = x.Value,
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
                    DisplayName = viewModel.DisplayName,
                    StatusId = viewModel.StatusId,
                    ModifiedBy = CurrentUser.Id
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
                DisplayName = bioDevice.DisplayName,
                StatusId = bioDevice.StatusId,
                Status = bioDevice.Status,
                Active = bioDevice.Active
            };

            return PartialView("_BioDeviceRow", viewModel);
        }

        public ActionResult BioTimeChange()
        {
            var viewModel = new BioStationTimeChangeViewModel
            {
                From = DateTime.Now.AddMonths(-1),
                To = DateTime.Now
            };

            return View(viewModel);
        }

        public ActionResult GetBioStationTimeChange(BioStationTimeChangeViewModel viewModel)
        {
            var criteria = new GetBioTimeChangeCriteria
            {
                FromDate = viewModel.From,
                ToDate = viewModel.To
            };

            var timeChanges = bioService.GetBioTimeChanges(criteria);

            return PartialView("_ListBioTimeChanges", timeChanges);
        }

        public ActionResult Fingerprint()
        {
            var viewModel = new FingerprintAndCardViewModel
            {
                Teams = teamService.GetAll()
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

            var listFingerprintData = bioService.GetFingerprintAndCardSummary(criteria);

            return PartialView("_ListFingerprint", listFingerprintData);
        }

        [HttpPost]
        public ActionResult ChangeFingerprintStatus(int employeeId, int bioUserId, FingerprintScannerStatus status)
        {
            var criteria = new UpdateFingerprintAndCardStatusCriteria
            {
                EmployeeId = employeeId,
                BioUserId = bioUserId,
                Status = status,
                UpdatedBy = CurrentUser.Id
            };

            var commandResult = commandService.Update(criteria);

            return Json(new { commandResult });
        }
    }
}