namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class MemoController : Controller
    {
        private readonly IMemoService _memoService;
        private readonly ILookupDataService _lookupDataService;
        private readonly ICommandService commandService;

        public MemoController()
            : this(null, null, null)
        {
        }

        public MemoController(IMemoService memoService, ILookupDataService lookupDataService, ICommandService commandService)
        {
            _memoService = memoService ?? new MemoService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            IList<LookupData> memoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
            memoCategories.Insert(0, new LookupData() { Name = "All", Id = -1 });
            var model = new MemoViewModel
            {
                Content = string.Empty,
                Year = -1,
                Category = -1,
                MemoCategories = memoCategories
            };

            return View(model);
        }

        public ActionResult Edit(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };

            Memo memo = _memoService.GetById(getMemoByIdCriteria);

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            memo.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);

            ViewBag.Updated = false;

            return View(memo);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Memo model)
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

            if (ModelState.IsValid)
            {
                var updateMemoCriteria = new UpdateMemoCriteria()
                {
                    Id = model.Id,
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo,
                    RefNo = model.RefNo,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = model.Subject,
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };
                CommandResult result = commandService.Update(updateMemoCriteria);
                if (result.Code == 0)
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
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
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
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.MemoCategory };
            model.MemoCategories = _lookupDataService.GetAll(getAllLookupDataCriteria);
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
                    ForInfo = model.ForInfo == null ? string.Empty : model.ForInfo,
                    RefNo = model.RefNo,
                    SendMail = model.SendMail,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = model.Subject,
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };

                CommandResult result = commandService.Insert(insertMemoCriteria, hasGetOutputParams: true);

                if (result.Code == 0)
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
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
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
        public bool DeleteMemo(int id)
        {
            var deleteMemoCriteria = new DeleteMemoCriteria() { Id = id };
            return commandService.Delete(deleteMemoCriteria).Code == 0;
        }
    }
}