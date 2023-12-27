namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;
    using Microsoft.AspNetCore.Mvc;

    public class WorkFromHomeRequestController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IWorkFromHomeRequest workFromHomeRequest;

        public WorkFromHomeRequestController(ISessionService sessionService,
            IBaseService baseService,
            IWorkFromHomeRequest workFromHomeRequest
            ) : base(sessionService)
        {
            this.baseService = baseService;
            this.workFromHomeRequest = workFromHomeRequest;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetWorkFromHomeRequestGrid(int year)
        {
            var criteria = new GetWorkFromHomeRequestCriteria
            {
                FromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
                EmpId = CurrentUser.Id,
            };

            var requests = await workFromHomeRequest.GetAll(criteria);

            foreach (var request in requests)
            {
                request.Status = GetStatusName(request.StatusId);
            }
            return PartialView("_WorkFromHomeRequestGrid", requests);
        }

        public IActionResult Create()
        {
            var model = new WorkFromHomeRequestViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(WorkFromHomeRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertWorkFromHomeRequestCriteria
                {
                    EmpId = CurrentUser.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await workFromHomeRequest.GetById(id);
            var model = new WorkFromHomeRequestViewModel
            {
                Id = request.Id,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Remark = request.Remark
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WorkFromHomeRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new EditWorkFromHomeRequestCriteria
                {
                    WorkFromHomeRequestId = model.Id,
                    EmpId = CurrentUser.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var criteria = new DeleteWorkFromHomeRequestCriteria { WorkFromHomeRequestId = id };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        private static string GetStatusName(byte Id)
        {
            const int secondCase = 2;
            const int threeCase = 3;

            switch (Id)
            {
                case 1:
                    return Labels.Pending;

                case secondCase:
                    return Labels.Approved;

                case threeCase:
                    return Labels.Rejected;

                default:
                    return string.Empty;
            }
        }
    }
}