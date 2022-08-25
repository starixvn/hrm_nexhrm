namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Bio;
    using Criteria.LookupData;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Models.Bio;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Bio;

    [HrmAuthorize(Roles = "HR")]
    public class BioController : BaseController
    {
        private readonly ICommandService commandService;

        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly IBioService bioService;

        public BioController(ISessionService sessionService,
            ICommandService commandService,

            ITeamService teamService,
            IBaseService baseService,
            IBioService bioService
            ) : base(sessionService)
        {
            this.commandService = commandService;
            this.teamService = teamService;
            this.baseService = baseService;
            this.bioService = bioService;
        }

        public async Task<ActionResult> BioDevice()
        {
            var bioDevices = await baseService.GetDataAsync<BioDevice>(new GetBioDevicesCriteria());
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

        public async Task<ActionResult> EditBioDevice(int id)
        {
            var bioDevice = await baseService.GetDataEntityAsync<BioDevice>(new GetBioDeviceCriteria { DeviceId = id });
            var bioDeviceStatuses = await baseService.GetDataAsync<LookupData>(new GetAllLookupDataCriteria { LookupCode = LookupCodeName.BioDeviceStatus });
            var viewModel = new EditBioDeviceViewModel
            {
                DeviceId = bioDevice.DeviceId,
                DisplayName = bioDevice.DisplayName,
                StatusId = bioDevice.StatusId,
                BioDeviceStatuses = bioDeviceStatuses
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

                var result = commandService.ExecuteNonQuery(criteria);

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
        public async Task<ActionResult> ReloadBioDevice(int id)
        {
            var bioDevice = await baseService.GetDataEntityAsync<BioDevice>(new GetBioDeviceCriteria { DeviceId = id });
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

        public async Task<ActionResult> GetBioStationTimeChange(BioStationTimeChangeViewModel viewModel)
        {
            var criteria = new GetBioTimeChangeCriteria
            {
                FromDate = viewModel.From,
                ToDate = viewModel.To
            };
            var timeChanges = await bioService.GetBioTimeChanges(criteria);
            return PartialView("_ListBioTimeChanges", timeChanges);
        }

        public async Task<ActionResult> Fingerprint()
        {
            var viewModel = new FingerprintAndCardViewModel
            {
                Teams = await teamService.GetAll()
            };
            return View(viewModel);
        }

        public async Task<ActionResult> GetFingerprintScannerData(FingerprintAndCardViewModel viewModel)
        {
            var criteria = new GetFingerprintAndCardSummaryCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                GroupId = viewModel.TeamId,
                Type = viewModel.Type,
                Status = viewModel.Status
            };

            var listFingerprintData = await bioService.GetFingerprintAndCardSummary(criteria);
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

            var commandResult = commandService.ExecuteNonQuery(criteria);

            return Json(new { commandResult });
        }
    }
}