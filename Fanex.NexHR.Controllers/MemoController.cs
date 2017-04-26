namespace Fanex.NexHR.Controllers
{
    using Criteria.LookupData;
    using Criteria.Memo;
    using Models;
    using Models.Memo;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using ViewModels.Memo;

    public class MemoController : BaseController
    {
        private readonly IMemoService _memoService;
        private readonly ILookupDataService _lookupDataService;
        private readonly ICommandService _commandService;

        public MemoController()
            : this(null, null, null)
        {
        }

        public MemoController(IMemoService memoService, ILookupDataService lookupDataService, ICommandService commandService)
        {
            _memoService = memoService ?? new MemoService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.MemoCategory
            };
            IList<LookupData> memoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            memoCategories.Insert(0, new LookupData() { Name = "All", Id = -1 });
            var model = new MemoViewModel
            {
                Content = string.Empty,
                Year = -1,
                Category = -1,
                MemoCategories = memoCategories,
                Status = 1
            };

            return View(model);
        }

        public ActionResult Edit(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria
            {
                Id = id
            };

            Memo memo = _memoService.GetById(getMemoByIdCriteria);

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.MemoCategory
            };

            memo.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);

            ViewBag.Updated = false;

            return View(memo);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Memo model)
        {
            if (model != null && ModelState.IsValid)
            {
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
                model.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
                if (model.MemoType == 1)
                {
                    model.Contents = model.AttachFile;
                    if (string.IsNullOrEmpty(model.AttachFile))
                    {
                        return View(model);
                    }
                }

                var updateMemoCriteria = new UpdateMemoCriteria()
                {
                    Id = model.Id,
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo ?? string.Empty,
                    RefNo = model.RefNo,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = HttpUtility.HtmlEncode(model.Subject),
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };
                CommandResult result = _commandService.Update(updateMemoCriteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    if (model.SendMail)
                    {
                        _memoService.SendMail(updateMemoCriteria.Subject, updateMemoCriteria.Id.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                MemoType = 1
            };

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            model.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);

            ViewBag.Updated = false;

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Memo model)
        {
            if (model != null && ModelState.IsValid)
            {
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
                model.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
                if (model.MemoType == 1)
                {
                    model.Contents = model.AttachFile;
                }

                var insertMemoCriteria = new InsertMemoCriteria()
                {
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo ?? string.Empty,
                    RefNo = model.RefNo,
                    SendMail = model.SendMail,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = HttpUtility.HtmlEncode(model.Subject),
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };

                CommandResult result = _commandService.Insert(insertMemoCriteria, hasGetOutputParams: true);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    if (insertMemoCriteria.SendMail)
                    {
                        _memoService.SendMail(insertMemoCriteria.Subject, insertMemoCriteria.MemoId.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        public ActionResult Detail(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = _memoService.GetById(getMemoByIdCriteria);

            return View(memo);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteMemo(int id)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = _memoService.GetById(getMemoByIdCriteria);

            var deleteMemoCriteria = new DeleteMemoCriteria() { Id = id };
            CommandResult result = _commandService.Delete(deleteMemoCriteria);
            if (result.IsSuccessful && memo.MemoType == 1)
            {
                System.IO.File.Delete(Server.MapPath("~/" + memo.Contents));
            }

            return Json(result);
        }

        public JsonResult GetMemos(int category, int year, int status)
        {
            GetAllMemoCriteria criteria = new GetAllMemoCriteria()
            {
                Category = category,
                Year = year,
                Status = Roles.IsUserInRole("HR") ? status : 1
            };

            IList<Memo> memos = _memoService.GetAll(criteria);

            return Json(memos);
        }
    }
}