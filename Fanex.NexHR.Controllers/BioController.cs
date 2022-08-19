namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enums;
    using Criteria.Bio;
    using Criteria.LookupData;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Authorization;
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
            //var bioDevices = await baseService.GetDataAsync<BioDevice>(new GetBioDevicesCriteria());
            var bioDevices = new List<BioDevice>
            {
                new BioDevice
                {
                    DeviceId = 1,
                    DeviceName= "DeviceName",
                    DisplayName = "DisplayName",
                    StatusId = "StatusId",
                    Status = "Status",
                    Active = true
                }
            };
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
            //var bioDevice = await baseService.GetDataEntityAsync<BioDevice>(new GetBioDeviceCriteria { DeviceId = id });
            var bioDevice = new BioDevice
            {
                DeviceId = 1,
                DeviceName = "DeviceName",
                DisplayName = "DisplayName",
                StatusId = "StatusId",
                Status = "Status",
                Active = true
            };
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
            //var timeChanges = await bioService.GetBioTimeChanges(criteria);
            var timeChanges = new List<BioTimeChange>
            {
                new BioTimeChange
                {
                    DeviceId = 1,
                    DeviceName = "DeviceName",
                    DisplayName ="DisplayName",
                    TimeChange = new DateTime(),
                    PreviousLogTime = new DateTime(),
                    NextLogTime = new DateTime(),
                }
            };
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

        public async Task<ActionResult> GetFingerprintScannerData(FingerprintAndCardViewModel viewModel)
        {
            var criteria = new GetFingerprintAndCardSummaryCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                GroupId = viewModel.TeamId,
                Type = viewModel.Type,
                Status = viewModel.Status
            };

            //var listFingerprintData = await bioService.GetFingerprintAndCardSummary(criteria);

            var listFingerprintData = new List<FingerprintAndCardSummary>
            {
                new FingerprintAndCardSummary
                {
                    Employee = new Employee
                    {
                        Id = 1,
                        Name = "test",
                        NickName = "test",
                        FullName="test",
                        Team = new Team
                        {
                            Name = "team name"
                        }
                    },
                    BioUserId = 1,
                    TotalFingerprint = 1,
                    CardNo = "CardNo",
                    IsFingerprintWarning = false,
                    IsCardWarning = false,
                    FullName ="FullName",
                    NickName = "NickName"
                }
            };
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