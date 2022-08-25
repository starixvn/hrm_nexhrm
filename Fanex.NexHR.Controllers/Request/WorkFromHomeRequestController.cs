namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;
    using Microsoft.AspNetCore.Mvc;

    public class WorkFromHomeRequestController : BaseController
    {
        private readonly ICommandService commandService;
        private readonly IWorkFromHomeRequest workFromHomeRequest;

        public WorkFromHomeRequestController(ISessionService sessionService,
            ICommandService commandService,
            IWorkFromHomeRequest workFromHomeRequest
            ) : base(sessionService)
        {
            this.commandService = commandService;
            this.workFromHomeRequest = workFromHomeRequest;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetWorkFromHomeRequestGrid(int year)
        {
            var criteria = new GetWorkFromHomeRequestCriteria
            {
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmpId = CurrentUser.Id,
            };

            var requests = await workFromHomeRequest.GetAll(criteria);

            foreach(var request in requests)
            {
                request.Status = GetStatusName(request.StatusId);
            }
            return PartialView("_WorkFromHomeRequestGrid", requests);
        }

        public ActionResult Create()
        {
            var model = new WorkFromHomeRequestViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(WorkFromHomeRequestViewModel model)
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

                var result = commandService.ExecuteNonQuery(criteria);
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

        public async Task<ActionResult> Edit(int id)
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
        public ActionResult Edit(WorkFromHomeRequestViewModel model)
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

                var result = commandService.ExecuteNonQuery(criteria);
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
        public ActionResult Delete(int id)
        {
            var criteria = new DeleteWorkFromHomeRequestCriteria { WorkFromHomeRequestId = id };
            var result = commandService.ExecuteNonQuery(criteria);

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
