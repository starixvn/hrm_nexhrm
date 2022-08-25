namespace Fanex.NexHR.Controllers.Request
{
    using Criteria.LateInEarlyOut;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Resources;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.LateInEarlyOut;

    public class LateEarlyController : BaseController
    {
        private readonly ILateInEarlyOutService lateInEarlyOutService;
        private readonly ICommandService commandService;

        public LateEarlyController(ISessionService sessionService, 
            ILateInEarlyOutService lateInEarlyOutService, 
            ICommandService commandService) : base(sessionService)
        {
            this.lateInEarlyOutService = lateInEarlyOutService;
            this.commandService = commandService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View(new LateInEarlyOutViewModel());
        }

        [HttpPost]
        public ActionResult Create(LateInEarlyOutViewModel model)
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
                    CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        public async Task<ActionResult> Edit(int id)
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
        public ActionResult Edit(LateInEarlyOutViewModel model)
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
                    CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        public JsonResult Delete(int id)
        {
            DeleteLateInEarlyOutCriteria criteria = new DeleteLateInEarlyOutCriteria { Id = id };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        public async Task<ActionResult> ListLateEarly(int empId, int year)
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
