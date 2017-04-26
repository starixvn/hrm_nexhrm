namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.CompanyEvent;
    using Models;
    using Models.CompanyEvent;
    using Resources;
    using Services;
    using Services.Interfaces;

    public class CompanyEventController : BaseController
    {
        private readonly ICommandService _commandService;
        private readonly ICompanyEventService _eventService;

        public CompanyEventController()
            : this(null, null)
        {
        }

        public CompanyEventController(ICommandService commandService, ICompanyEventService eventService)
        {
            _commandService = commandService ?? new CommandService();
            _eventService = eventService ?? new CompanyEventService();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Detail(int id)
        {
            CompanyEvent model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria() { Id = id };
                model = _eventService.GetById(criteria);
            }

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        public JsonResult GetAllEvents()
        {
            var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-6);

            var lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).AddMonths(7);

            GetAllEventCriteria criteria = new GetAllEventCriteria()
            {
                FromDate = firstDay,
                ToDate = lastDay
            };
            IList<CompanyEvent> events = _eventService.GetAll(criteria);
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
                    FromTime = model.FromTime ?? string.Empty,
                    ToTime = model.ToTime ?? string.Empty,
                    Location = model.Location,
                    Description = model.Description ?? string.Empty,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = _commandService.Insert(criteria);
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
                model = _eventService.GetById(criteria);
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
                    FromTime = model.FromTime ?? string.Empty,
                    ToTime = model.ToTime ?? string.Empty,
                    Location = model.Location,
                    Description = model.Description ?? string.Empty
                };
                CommandResult result = _commandService.Update(criteria);
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
                result = _commandService.Delete(criteria);
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
