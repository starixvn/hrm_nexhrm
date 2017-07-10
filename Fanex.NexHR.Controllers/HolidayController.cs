namespace Fanex.NexHR.Controllers
{
    using Criteria.Holiday;
    using Models;
    using Models.Holiday;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels.Holiday;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService _holidayService;
        private readonly ICommandService _commandService;

        public HolidayController()
            : this(null, null)
        {
        }

        public HolidayController(IHolidayService holidayService, ICommandService commandService)
        {
            _holidayService = holidayService ?? new HolidayService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            return View();
        }

        public JsonResult GetHoliday(int year)
        {
            var criteria = new GetHolidayByYearCriteria
            {
                Year = year
            };

            IList<Holiday> holidays = _holidayService.GetByYear(criteria);

            return Json(holidays, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
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

        [Authorize(Roles = "HR")]
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

                result = _commandService.Insert(criteria);

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

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetHolidayByIdCriteria
            {
                Id = id
            };
            Holiday holiday = _holidayService.GetById(criteria);

            var model = new HolidayViewModel
            {
                Id = holiday.Id,
                Name = holiday.Name,
                FromDate = holiday.FromDate,
                ToDate = holiday.ToDate
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
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

                    result = _commandService.Update(criteria);
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteHolidayCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = _commandService.Delete(criteria);

            return Json(result);
        }
    }
}