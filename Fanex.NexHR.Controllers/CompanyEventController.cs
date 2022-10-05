namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.CompanyEvent;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.CompanyEvent;
    using Resources;
    using Services.Interfaces;

    public class CompanyEventController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly ICompanyEventService eventService;

        public CompanyEventController(ISessionService sessionService,
            IBaseService baseService,
            ICompanyEventService eventService) : base(sessionService)
        {
            this.baseService = baseService;
            this.eventService = eventService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            return View(await GetDetail(id));
        }

        public async Task<JsonResult> GetAllEvents()
        {
            var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-6);

            var lastDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1).AddMonths(7);

            GetAllEventCriteria criteria = new GetAllEventCriteria
            {
                FromDate = firstDay,
                ToDate = lastDay
            };
            IEnumerable<CompanyEvent> events = await eventService.GetAll(criteria);
            return Json(events);
        }

        public IActionResult Create(DateTime from, DateTime to)
        {
            var companyEvent = new CompanyEvent
            {
                FromDate = from,
                ToDate = to
            };
            return View(companyEvent);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(CompanyEvent model)
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await GetDetail(id));
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Edit(CompanyEvent model)
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
                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            CommandResult result = new CommandResult();
            if (id > 0)
            {
                DeleteEventCriteria criteria = new DeleteEventCriteria { Id = id };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            else
            {
                result.Code = 1;
                result.Message = Messages.InvalidInputData;
            }

            return Json(result);
        }

        private async Task<CompanyEvent> GetDetail(int id)
        {
            CompanyEvent model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria { Id = id };
                model = await eventService.GetById(criteria);
            }

            return model;
        }
    }
}