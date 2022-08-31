namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Criteria.Memo;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Memo;
    using Services;
    using Services.Interfaces;
    using ViewModels.Memo;

    public class MemoController : BaseController
    {
        private readonly IMemoService memoService;
        private readonly ILookupDataService lookupDataService;
        private readonly IBaseService baseService;
        private readonly IWebHostEnvironment environment;
        private readonly IEmployeeService employeeService;

        public MemoController(ISessionService sessionService,
            IMemoService memoService,
            ILookupDataService lookupDataService,
            IBaseService baseService,
            IWebHostEnvironment environment,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.memoService = memoService;
            this.lookupDataService = lookupDataService;
            this.baseService = baseService;
            this.environment = environment;
            this.employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var getAllLookupDataCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = LookupCodeName.MemoCategory
            };
            IEnumerable<LookupData> memoCategories = await lookupDataService.GetAll(getAllLookupDataCriteria);
            var memoCategorieLst = memoCategories.ToList();
            memoCategorieLst.Insert(0, new LookupData { Name = "All", Id = -1 });
            var model = new MemoViewModel
            {
                Content = string.Empty,
                Year = -1,
                Category = -1,
                MemoCategories = memoCategorieLst,
                Status = 1
            };

            return View(model);
        }

        public async Task<IActionResult> Edit(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria
            {
                Id = id
            };

            Memo memo = await memoService.GetById(getMemoByIdCriteria);

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = LookupCodeName.MemoCategory
            };

            memo.MemoCategories = await lookupDataService.GetAll(getAllLookupDataCriteria);

            ViewBag.Updated = false;

            return View(memo);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Edit(Memo model)
        {
            if (model != null && ModelState.IsValid)
            {
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria { LookupCode = LookupCodeName.MemoCategory };
                model.MemoCategories = await lookupDataService.GetAll(getAllLookupDataCriteria);
                if (model.MemoType == 1)
                {
                    model.Contents = model.AttachFile;
                    if (string.IsNullOrEmpty(model.AttachFile))
                    {
                        return View(model);
                    }
                }

                var updateMemoCriteria = new UpdateMemoCriteria
                {
                    Id = model.Id,
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo ?? string.Empty,
                    RefNo = model.RefNo,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = WebUtility.HtmlEncode(model.Subject),
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };
                CommandResult result = await baseService.ExecuteNonQueryAsync(updateMemoCriteria);
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

        [HrmAuthorize(Roles = "HR")]
        public async Task<IActionResult> Create()
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

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria { LookupCode = LookupCodeName.MemoCategory };
            model.MemoCategories = await lookupDataService.GetAll(getAllLookupDataCriteria);

            ViewBag.Updated = false;

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(Memo model)
        {
            if (model != null && ModelState.IsValid)
            {
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria { LookupCode = LookupCodeName.MemoCategory };
                model.MemoCategories = await lookupDataService.GetAll(getAllLookupDataCriteria);
                if (model.MemoType == 1)
                {
                    model.Contents = model.AttachFile;
                }

                var insertMemoCriteria = new InsertMemoCriteria
                {
                    Category = model.Category,
                    Contents = model.Contents,
                    ForInfo = model.ForInfo ?? string.Empty,
                    RefNo = model.RefNo,
                    SendMail = model.SendMail,
                    SentDate = model.SentDate,
                    SentFrom = model.SentFrom,
                    SentTo = model.SentTo,
                    Subject = WebUtility.HtmlEncode(model.Subject),
                    TotalPages = model.TotalPages,
                    MemoType = model.MemoType,
                    Status = model.Status == 1
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(insertMemoCriteria);

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

        public async Task<IActionResult> Detail(int id = -1)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = await memoService.GetById(getMemoByIdCriteria);

            return View(memo);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<JsonResult> DeleteMemo(int id)
        {
            GetMemoByIdCriteria getMemoByIdCriteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = await memoService.GetById(getMemoByIdCriteria);

            var deleteMemoCriteria = new DeleteMemoCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteMemoCriteria);

            if (result.IsSuccessful && memo.MemoType == 1)
            {
                string[] files = memo.AttachFile.TrimEnd(';').Split(';');

                foreach (string file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        var path = Path.Combine(environment.WebRootPath, file);
                        System.IO.File.Delete(path);
                    }
                }
            }

            return Json(result);
        }

        public async Task<JsonResult> GetMemos(int category, int year, int status)
        {
            var userRoles = employeeService.UserRoles(CurrentUser);
            var isIsInRoleHR = userRoles.Any(item => (item == Role.HR.EnumToString()));

            GetAllMemoCriteria criteria = new GetAllMemoCriteria
            {
                Category = category,
                Year = year,
                Status = isIsInRoleHR ? status : 1
            };

            IEnumerable<Memo> memos = await memoService.GetAll(criteria);

            return Json(memos);
        }
    }
}