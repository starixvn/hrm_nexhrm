namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Postnatal;
    using Fanex.NexHR.Core.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using ViewModels.Postnatal;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService postnatalService;
        private readonly IBaseService baseService;
        private const int MixPostnatalTypeCount = 2;
        private const int ThirtyMinutes = 30;
        private const int SixtyMinutes = 60;

        public PostnatalController(ISessionService sessionService,
            IPostnatalService postnatalService,
            IBaseService baseService) : base(sessionService)
        {
            this.postnatalService = postnatalService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            PostnatalViewModel model = new PostnatalViewModel
            {
                EmpId = CurrentUser.Id,
                FromDate = DateTimeOffset.Now.DateTime,
                ToDate = DateTimeOffset.Now.DateTime,
                Types = 0,
                Minutes = ThirtyMinutes
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostnatalViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                InsertPostnatalCriteria criteria = new InsertPostnatalCriteria
                {
                    EmployeeId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Minutes = GetMinuteByType(model),
                    Types = model.ChosenTypes.Count == MixPostnatalTypeCount ? (int)PostnatalType.MixLateInEarlyOutAndBreakTime : model.ChosenTypes[0]
                };
                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int empId, string from, string to)
        {
            GetByIdPostnatalCriteria criteria = new GetByIdPostnatalCriteria
            {
                EmployeeId = empId,
                FromDate = DateTime.Parse(from, CultureInfo.InvariantCulture),
                ToDate = DateTime.Parse(to, CultureInfo.InvariantCulture)
            };
            PostnatalViewModel employee = await postnatalService.GetById(criteria);
            if (employee != null)
            {
                employee.OldEmpId = employee.EmpId;
                employee.OldFromDate = employee.FromDate;
                employee.OldToDate = employee.ToDate;
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostnatalViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                UpdatePostnatalCriteria criteria = new UpdatePostnatalCriteria
                {
                    OldEmployeeId = model.OldEmpId,
                    OldFromDate = model.OldFromDate,
                    OldToDate = model.OldToDate,
                    NewEmployeeId = model.EmpId,
                    NewFromDate = model.FromDate,
                    NewToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Minutes = GetMinuteByType(model),
                    Types = model.ChosenTypes.Count == MixPostnatalTypeCount ? (int)PostnatalType.MixLateInEarlyOutAndBreakTime : model.ChosenTypes[0]
                };
                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int empId, DateTime fromDate, DateTime toDate)
        {
            DeletePostnatalCriteria criteria = new DeletePostnatalCriteria
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        public async Task<IActionResult> ListPostnatalEmployee(int empId, int year)
        {
            GetAllPostnatalCriteria criteria = new GetAllPostnatalCriteria
            {
                EmpId = empId,
                Year = year
            };
            IEnumerable<PostnatalViewModel> listPostnatal = await postnatalService.GetAll(criteria);

            return PartialView("_ListPostnatalEmployee", listPostnatal.ToList());
        }

        private static int GetMinuteByType(PostnatalViewModel model)
        {
            if (model.ChosenTypes.Count == MixPostnatalTypeCount)
            {
                return ThirtyMinutes;
            }

            return model.ChosenTypes[0] == (int)PostnatalType.LateInEarlyOut ? SixtyMinutes : 0;
        }
    }
}