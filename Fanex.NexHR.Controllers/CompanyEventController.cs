namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using Criteria.CompanyEvent;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.CompanyEvent;
    using Resources;
    using Services.Interfaces;

    public class CompanyEventController : BaseController
    {
        private readonly ICommandService commandService;
        private readonly ICompanyEventService eventService;

        public CompanyEventController(ISessionService sessionService, 
            ICommandService commandService, 
            ICompanyEventService eventService) : base(sessionService)
        {
            this.commandService = commandService;
            this.eventService = eventService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int id)
        {
            return View(GetDetail(id));
        }

        public JsonResult GetAllEvents()
        {
            var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-6);

            var lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).AddMonths(7);

            GetAllEventCriteria criteria = new GetAllEventCriteria
            {
                FromDate = firstDay,
                ToDate = lastDay
            };
            IList<CompanyEvent> events = eventService.GetAll(criteria);
            return Json(events);
        }

        public ActionResult Create(DateTime from, DateTime to)
        {
            var companyEvent = new CompanyEvent
            {
                FromDate = from,
                ToDate = to
            };
            return View(companyEvent);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(CompanyEvent model)
        {
            if (model != null && ModelState.IsValid)
            {
                InsertEventCriteria criteria = new InsertEventCriteria
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

                CommandResult result = commandService.ExecuteNonQuery(criteria);
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
            return View(GetDetail(id));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(CompanyEvent model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEventCriteria
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
                CommandResult result = commandService.ExecuteNonQuery(criteria);
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
                DeleteEventCriteria criteria = new DeleteEventCriteria { Id = id };
                result = commandService.ExecuteNonQuery(criteria);
            }
            else
            {
                result.Code = 1;
                result.Message = Messages.InvalidInputData;
            }

            return Json(result);
        }

        private CompanyEvent GetDetail(int id)
        {
            CompanyEvent model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria { Id = id };
                model = eventService.GetById(criteria);
            }

            return model;
        }
    }
}
