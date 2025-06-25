namespace Fanex.NexHR.Controllers
{
    using System;
    using Fanex.NexHR.Core;
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

        public CompanyEventController(
            ISessionService sessionService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
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
            var firstDay = new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, DateTimeOffset.Now.DateTime.ToClientTimeZone().Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(-6).DateTime;

            var lastDay = new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, DateTimeOffset.Now.DateTime.ToClientTimeZone().Month, 1, 0, 0, 0, TimeSpan.Zero).AddDays(-1).AddMonths(7).DateTime;

            var criteria = new GetAllEventCriteria
            {
                FromDate = firstDay,
                ToDate = lastDay
            };
            var events = await baseService.GetDataAsync<CompanyEvent>(criteria);
            
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
                var command = new InsertEventCommand
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(command);
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
                var command = new UpdateEventCommand
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
                CommandResult result = await baseService.ExecuteNonQueryAsync(command);
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
            var result = new CommandResult();
            if (id > 0)
            {
                var command = new DeleteEventCommand { Id = id };
                result = await baseService.ExecuteNonQueryAsync(command);
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
            var model = new CompanyEvent();
            if (id > 0)
            {
                var criteria = new GetEventByIdCriteria { Id = id };
                model = await baseService.GetDataEntityAsync<CompanyEvent>(criteria);
            }

            return model;
        }
    }
}