namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Holiday;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Holiday;
    using Resources;
    using Services.Interfaces;
    using ViewModels.Holiday;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly ICommandService commandService;

        public HolidayController(ISessionService sessionService, 
            IHolidayService holidayService, 
            ICommandService commandService) : base(sessionService)
        {
            this.holidayService = holidayService;
            this.commandService = commandService;
        }

        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
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

        [HrmAuthorize(Roles = "HR")]
        public ActionResult Create()
        {
            HolidayViewModel newHoliday = new HolidayViewModel
            {
                Name = string.Empty,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
            };

            return View(newHoliday);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(HolidayViewModel holiday)
        {            
            CommandResult result = new CommandResult();
            if (holiday != null && ModelState.IsValid)
            {
                var criteria = new InsertNewHolidayCriteria
                {
                    HolidayName = holiday.Name,
                    FromDate = holiday.FromDate,
                    ToDate = holiday.ToDate,
                    ModifiedBy = CurrentUser.Id
                };

                result = commandService.ExecuteNonQuery(criteria);

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

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Edit(int id)
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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(HolidayViewModel holiday)
        {
            CommandResult result = new CommandResult();
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

                    result = commandService.ExecuteNonQuery(criteria);
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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }
    }
}