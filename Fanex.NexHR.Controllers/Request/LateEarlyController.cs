namespace Fanex.NexHR.Controllers.Request
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.LateInEarlyOut;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Resources;
    using Services.Interfaces;
    using ViewModels.LateInEarlyOut;

    public class LateEarlyController : BaseController
    {
        private readonly ILateInEarlyOutService lateInEarlyOutService;
        private readonly IBaseService baseService;

        public LateEarlyController(ISessionService sessionService,
            ILateInEarlyOutService lateInEarlyOutService,
            IBaseService baseService) : base(sessionService)
        {
            this.lateInEarlyOutService = lateInEarlyOutService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new LateInEarlyOutViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(LateInEarlyOutViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (model.EmpId != 0)
                {
                    InsertLateInEarlyOutCriteria criteria = new InsertLateInEarlyOutCriteria
                    {
                        EmpId = model.EmpId,
                        Types = model.TypeCode,
                        Minutes = model.Minutes,
                        WorkDate = model.WorkDate,
                        ModifiedBy = CurrentUser.Id
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
                else
                {
                    ModelState.AddModelError(string.Empty, Labels.InvalidEmployee);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            LateInEarlyOutViewModel model = new LateInEarlyOutViewModel();
            GetLateInEarlyOutByIdCriteria criteria = new GetLateInEarlyOutByIdCriteria
            {
                Id = id
            };

            if (id > 0)
            {
                model = await lateInEarlyOutService.GetById(criteria);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LateInEarlyOutViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                if (model.EmpId != 0)
                {
                    UpdateLateInEarlyOutCriteria criteria = new UpdateLateInEarlyOutCriteria
                    {
                        Id = (int)model.Id,
                        EmpId = model.EmpId,
                        Types = int.Parse(model.Types),
                        Minutes = model.Minutes,
                        WorkDate = model.WorkDate,
                        ModifiedBy = CurrentUser.Id
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
                else
                {
                    ModelState.AddModelError(string.Empty, Labels.InvalidEmployee);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            DeleteLateInEarlyOutCriteria criteria = new DeleteLateInEarlyOutCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        public async Task<IActionResult> ListLateEarly(int empId, int year)
        {
            IEnumerable<LateInEarlyOutViewModel> model = await GetLateEarlyByEmpId(empId, year);
            return PartialView("_ListLateEarly", model.ToList());
        }

        private async Task<IEnumerable<LateInEarlyOutViewModel>> GetLateEarlyByEmpId(int empId, int year)
        {
            var criteria = new GetLateInEarlyOutByEmpIdCriteria
            {
                EmpId = empId,
                Year = year
            };
            IEnumerable<LateInEarlyOutViewModel> list = await lateInEarlyOutService.GetAllByEmpId(criteria);

            return list;
        }
    }
}