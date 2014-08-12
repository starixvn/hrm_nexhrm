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
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
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
            var getAllMemoCriteria = new GetAllMemoCriteria()
            {
                Category = model.Category,
                Content = model.Content,
                Year = model.Year
            };
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            ViewBag.EntityList = _memoService.GetAll(getAllMemoCriteria);

            return View(model);
        }

        /// <summary>
        /// Edit memo page.
        /// </summary>
        /// <param name="id">The memo ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Edit(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            Memo memo = _memoService.GetById(getMemoByIdCriteria);
            ViewBag.Updated = false;

            return View(memo);
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            if (model.MemoType == 1 && string.IsNullOrEmpty(model.AttachFile))
            {
                return View(model);
            }

            if (model.MemoType == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                var updateMemoCriteria = new UpdateMemoCriteria()
                {
                    Id = model.Id,
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo,
                    RefNo = model.RefNo,
                    SendMail = model.SendMail,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = model.Subject,
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType
                };
                ViewBag.Updated = _memoService.Update(updateMemoCriteria);
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            ViewBag.Updated = false;
            return View(new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                MemoType = 1
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            if (model.MemoType == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                var insertMemoCriteria = new InsertMemoCriteria()
                {
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo,
                    RefNo = model.RefNo,
                    SendMail = model.SendMail,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = model.Subject,
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType
                };
                var id = _memoService.Insert(insertMemoCriteria);

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
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = _memoService.GetById(getMemoByIdCriteria);
           
            return View(memo);
        }
    }
}