namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System.Web.Mvc;

    [Authorize]
    public class OverTimeController : BaseController
    {
        private readonly IOverTimeService _service;
        private readonly IEmployeeService _employeeService;

        public OverTimeController() : this(null, null) { }

        public OverTimeController(IOverTimeService service,
            IEmployeeService employeeService = null)
        {
            _service = service ?? new OverTimeService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create()
        {
            var model = new OverTimeViewModel();

            return View("Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create(OverTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedId = this.CurrentUser.Id;
                if (model.IsApprove)
                {
                    model.Status = (int)RequestStatuse.Approve;
                    _service.InsertAndApprove(model);
                }
                else
                {
                    model.Status = (int)RequestStatuse.Pending;
                    _service.Insert(model);
                }

                return RedirectToAction("Summary");
            }

            return View();
        }

        [HttpPost]
        public ActionResult SummaryResult(OverTimeSummaryFilterViewModel model)
        {
            var result = _service.GetSummaryReport(model);

            return View("SummaryResult", result);
        }

        public ActionResult Summary(OverTimeSummaryFilterViewModel model)
        {
            if (model.EmployeeId != 0)
            {
                var criteria = new GetEmployeeCriteria() { Id = model.EmployeeId };
                model.Employee = _employeeService.GetEmployeeById(criteria).DisplayName;
            }

            return View("Summary", model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult ApproveRequest(int id)
        {
            var result = _service.ApproveRequest(id, this.CurrentUser.Id);

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult RejectRequest(int id)
        {
            var result = _service.RejectRequest(id, this.CurrentUser.Id);

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult DisapproveRequest(int id)
        {
            var result = _service.DisapproveRequest(id, this.CurrentUser.Id);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(int id)
        {
            var model = _service.Get(id);

            return View(OverTimeViewModel.Parse(model));
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(OverTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedId = this.CurrentUser.Id;
                var result = _service.Update(model);

                return RedirectToAction("Summary");
            }

            return View();
        }
    }
}
