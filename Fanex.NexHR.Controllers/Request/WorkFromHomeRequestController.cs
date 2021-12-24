namespace Fanex.NexHR.Controllers.Request
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Request;

    public class WorkFromHomeRequestController : BaseController
    {
        private readonly ICommandService commandService;
        private readonly IWorkFromHomeRequest workFromHomeRequest;
        public WorkFromHomeRequestController() : this(null, null)
        {
        }

        public WorkFromHomeRequestController(
            ICommandService commandService,
            IWorkFromHomeRequest workFromHomeRequest
            )
        {
            this.commandService = commandService ?? new CommandService();
            this.workFromHomeRequest = workFromHomeRequest ?? new WorkFromHomeRequestService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetWorkFromHomeRequestGrid(int year)
        {
            var criteria = new GetWorkFromHomeRequestCriteria
            {
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31),
                EmpId = CurrentUser.Id,
            };

            var requests = workFromHomeRequest.GetAll(criteria);

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

        [ValidateInput(false)]
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

                var result = workFromHomeRequest.Insert(criteria);
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

        public ActionResult Edit(int id)
        {
            var request = workFromHomeRequest.GetById(id);
            var model = new WorkFromHomeRequestViewModel
            {
                Id = request.Id,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Remark = request.Remark
            };
            return View(model);
        }

        [ValidateInput(false)]
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

                var result = workFromHomeRequest.Update(criteria);
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
            var result = workFromHomeRequest.Delete(criteria);

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
