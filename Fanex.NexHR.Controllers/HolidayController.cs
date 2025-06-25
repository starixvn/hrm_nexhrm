namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Holiday;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Holiday;
    using Resources;
    using Services.Interfaces;
    using ViewModels.Holiday;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly IBaseService baseService;

        public HolidayController(ISessionService sessionService,
            IHolidayService holidayService,
            IBaseService baseService) : base(sessionService)
        {
            this.holidayService = holidayService;
            this.baseService = baseService;
        }

        public IActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTimeOffset.Now.DateTime.ToClientTimeZone().Year;
            return View();
        }

        public async Task<JsonResult> GetHoliday(int year)
        {
            var criteria = new GetHolidayByYearCriteria
            {
                Year = year
            };

            IEnumerable<Holiday> holidays = await holidayService.GetByYear(criteria);

            return Json(holidays);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Create()
        {
            var newHoliday = new HolidayViewModel
            {
                Name = string.Empty,
                FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone() 
            };

            return View(newHoliday);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(HolidayViewModel holiday)
        {
            var result = new CommandResult();
            if (holiday != null && ModelState.IsValid)
            {
                var criteria = new InsertNewHolidayCriteria
                {
                    HolidayName = holiday.Name,
                    FromDate = holiday.FromDate,
                    ToDate = holiday.ToDate,
                    ModifiedBy = CurrentUser.Id
                };

                result = await baseService.ExecuteNonQueryAsync(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var criteria = new GetHolidayByIdCriteria
            {
                Id = id
            };
            Holiday holiday = await holidayService.GetById(criteria);

            var model = new HolidayViewModel
            {
                Id = holiday.Id,
                Name = holiday.Name,
                FromDate = holiday.FromDate,
                ToDate = holiday.ToDate
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Edit(HolidayViewModel holiday)
        {
            var result = new CommandResult();
            if (holiday != null)
            {
                if (ModelState.IsValid)
                {
                    var criteria = new UpdateHolidayCriteria
                    {
                        Id = holiday.Id,
                        HolidayName = holiday.Name,
                        FromDate = holiday.FromDate,
                        ToDate = holiday.ToDate,
                        ModifiedBy = CurrentUser.Id,
                    };

                    result = await baseService.ExecuteNonQueryAsync(criteria);
                    if (result.IsSuccessful)
                    {
                        ViewBag.Updated = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
                }
            }

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }
    }
}