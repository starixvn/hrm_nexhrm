namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.CompanyEvent;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.CompanyEvent;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class CompanyEventController : BaseController
    {
        private readonly ICommandService commandService;
        private readonly ICompanyEventService eventService;

        public CompanyEventController()
            : this(null, null)
        {
        }

        public CompanyEventController(ICommandService commandService, ICompanyEventService eventService)
        {
            this.commandService = commandService ?? new CommandService();
            this.eventService = eventService ?? new CompanyEventService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            CompanyEvent model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria() { Id = id };
                model = eventService.GetById(criteria);
            }
            return View(model);
        }

        public JsonResult GetAllEvents()
        {
            var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-6);
            var lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddDays(-1).AddMonths(6);

            GetAllEventCriteria criteria = new GetAllEventCriteria()
            {
                FromDate = firstDay,
                ToDate = lastDay
            };
            IList<CompanyEvent> events = eventService.GetAll(criteria);
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(DateTime from, DateTime to)
        {
            var companyEvent = new CompanyEvent()
            {
                FromDate = from,
                ToDate = to
            };
            return View(companyEvent);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CompanyEvent model)
        {
            if (model != null && ModelState.IsValid)
            {
                InsertEventCriteria criteria = new InsertEventCriteria()
                {
                    Title = model.Title,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    FromTime = model.FromTime == null ? string.Empty : model.FromTime,
                    ToTime = model.ToTime == null ? string.Empty : model.ToTime,
                    Location = model.Location,
                    Description = model.Description == null ? string.Empty : model.Description,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Insert = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            CompanyEvent model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria() { Id = id };
                model = eventService.GetById(criteria);
            }
            return View(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(CompanyEvent model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEventCriteria()
                {
                    Id = model.Id,
                    Title = model.Title,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    FromTime = model.FromTime == null ? string.Empty : model.FromTime,
                    ToTime = model.ToTime == null ? string.Empty : model.ToTime,
                    Location = model.Location,
                    Description = model.Description == null ? string.Empty : model.Description
                };
                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public JsonResult Delete(int id)
        {
            CommandResult result = new CommandResult();
            if (id > 0)
            {
                DeleteEventCriteria criteria = new DeleteEventCriteria() { Id = id };
                result = commandService.Delete(criteria);
            }
            else
            {
                result.Code = 1;
                result.Message = Messages.InvalidInputData;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
