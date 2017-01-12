namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Memo;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Memo;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class MemoController : BaseController
    {
        private readonly IMemoService memoService;
        private readonly ILookupDataService lookupDataService;
        private readonly ICommandService commandService;

        public MemoController()
            : this(null, null, null)
        {
        }

        public MemoController(IMemoService memoService, ILookupDataService lookupDataService, ICommandService commandService)
        {
            this.memoService = memoService ?? new MemoService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.MemoCategory
            };
            IList<LookupData> memoCategories = lookupDataService.GetAll(getAllLookupDataCriteria);
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

            Memo memo = memoService.GetById(getMemoByIdCriteria);

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria()
            {
                LookupCode = LookupCodeName.MemoCategory
            };

            memo.MemoCategories = lookupDataService.GetAll(getAllLookupDataCriteria);

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
                model.MemoCategories = lookupDataService.GetAll(getAllLookupDataCriteria);
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
                    ForInfo = model.ForInfo == null ? string.Empty : model.ForInfo,
                    RefNo = model.RefNo,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = HttpUtility.HtmlEncode(model.Subject),
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };
                CommandResult result = commandService.Update(updateMemoCriteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    if (model.SendMail)
                    {
                        memoService.SendMail(updateMemoCriteria.Subject, updateMemoCriteria.Id.ToString());
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
            model.MemoCategories = lookupDataService.GetAll(getAllLookupDataCriteria);

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
                model.MemoCategories = lookupDataService.GetAll(getAllLookupDataCriteria);
                if (model.MemoType == 1)
                {
                    model.Contents = model.AttachFile;
                }

                var insertMemoCriteria = new InsertMemoCriteria()
                {
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo == null ? string.Empty : model.ForInfo,
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

                CommandResult result = commandService.Insert(insertMemoCriteria, hasGetOutputParams: true);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                    if (insertMemoCriteria.SendMail)
                    {
                        memoService.SendMail(insertMemoCriteria.Subject, insertMemoCriteria.MemoId.ToString());
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
            Memo memo = memoService.GetById(getMemoByIdCriteria);

            return View(memo);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteMemo(int id)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = memoService.GetById(getMemoByIdCriteria);

            var deleteMemoCriteria = new DeleteMemoCriteria() { Id = id };
            CommandResult result = commandService.Delete(deleteMemoCriteria);
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

            IList<Memo> memos = memoService.GetAll(criteria);

            return Json(memos);
        }
    }
}