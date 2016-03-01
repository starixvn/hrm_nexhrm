namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Holiday;

    public class HolidayController : BaseController
    {
        private readonly IHolidayService holidayService;
        private readonly ICommandService commandService;

        public HolidayController()
            : this(null, null)
        {
        }

        public HolidayController(IHolidayService holidayService, ICommandService commandService)
        {
            this.holidayService = holidayService ?? new HolidayService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            return View();
        }

        public JsonResult GetHoliday(int year)
        {
            var criteria = new GetHolidayByYearCriteria()
            {
                Year = year
            };

            IList<Holiday> holidays = holidayService.GetByYear(criteria);

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

                result = commandService.Insert(criteria);

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
            var criteria = new GetHolidayByIdCriteria()
            {
                Id = id
            };
            Holiday holiday = holidayService.GetById(criteria);

            var model = new HolidayViewModel()
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

                    result = commandService.Update(criteria);
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
            var criteria = new DeleteHolidayCriteria()
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }
    }
}