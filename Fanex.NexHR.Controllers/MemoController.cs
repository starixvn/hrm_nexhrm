// --------------------------------------------------------------------------
// <copyright file="MemoController.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Memo controller.
    /// </summary>
    [Authorize]
    public class MemoController : Controller
    {
        private readonly IMemoService _memoService;
        private readonly ILookupDataService _lookupDataService;

        public MemoController(IMemoService memoService = null, ILookupDataService lookupDataService = null)
        {
            _memoService = memoService ?? new MemoService();
            _lookupDataService = lookupDataService ?? new LookupDataService();            
        }

        public MemoController()
            : this(null)
        { 
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            MemoViewModel model = new MemoViewModel
            {
                Content = string.Empty,
                Year = DateTime.Now.Year,
                Category = -1
            };

            return Index(model);
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Index(MemoViewModel model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            ViewBag.EntityList = _memoService.GetAll(model);
            return View(model);
        }

        /// <summary>
        /// Edit memo page.
        /// </summary>
        /// <param name="id">The memo ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Edit(int id = -1)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            return View(_memoService.GetById(id));
        }

        /// <summary>
        /// Edit memo page.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Memo model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            if (model.Type == 1 && string.IsNullOrEmpty(model.AttachFile))
            {
                return View(model);
            }

            if (model.Type == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                ViewBag.Updated = _memoService.Update(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// Create memo.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            return View(new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                Type = 1
            });
        }

        /// <summary>
        /// Create memo.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Memo model)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            if (model.Type == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                var id = _memoService.Insert(model);

                ViewBag.Updated = id != -1 ? true : false;
                if (id == -1)
                {
                    ModelState.AddModelError(string.Empty, "Duplicate memo");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// View memo detail.
        /// </summary>
        /// <param name="id">The memo ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Detail(int id = -1)
        {
            return View(_memoService.GetById(id));
        }
    }
}