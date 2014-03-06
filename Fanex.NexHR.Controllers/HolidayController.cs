// --------------------------------------------------------------------------
// <copyright file="HolidayController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Holiday;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Holiday controller.
    /// </summary>
    [Authorize]
    public class HolidayController : BaseController
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService = null)
        {
            _holidayService = holidayService ?? new HolidayService();
        }

        public HolidayController()
            : this(null)
        { 
        }

        public ActionResult Index()
        {
            return View(_holidayService.GetByYear(DateTime.Now.Year));
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="year">The year value.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult Index(int? year)
        {
            ViewBag.year = year ?? DateTime.Now.Year;
            return View(_holidayService.GetByYear(ViewBag.year));
        }

        /// <summary>
        /// Edit holiday.
        /// </summary>
        /// <param name="id">The holiday ID.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            return View(_holidayService.GetById(id));
        }

        /// <summary>
        /// Edit holiday.
        /// </summary>
        /// <param name="holiday">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(Holiday holiday)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateHolidayCriteria
                {
                    Id = holiday.Id,
                    HolidayName = holiday.HolidayName,
                    FromDate = holiday.FromDate,
                    ToDate = holiday.ToDate,
                    ModifiedBy = this.CurrentUser.Id,
                    ModifiedDate = DateTime.Now
                };

                ViewBag.Updated = _holidayService.Update(criteria);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(holiday);
        }

        /// <summary>
        /// Create holiday.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            return View(new Holiday
            {
                Name = string.Empty,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ModifiedBy = this.CurrentUser.Id
            });
        }

        /// <summary>
        /// Create holiday.
        /// </summary>
        /// <param name="holiday">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(Holiday holiday)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertNewHolidayCriteria
                {
                    HolidayName = holiday.HolidayName,
                    FromDate = holiday.FromDate,
                    ToDate = holiday.ToDate,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };

                var id = _holidayService.Insert(criteria);

                if (id == -1)
                {
                    ViewBag.Updated = false;
                    ModelState.AddModelError(string.Empty, "Duplicate holiday");
                }
                else
                {
                    ViewBag.Updated = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(holiday);
        }
    }
}