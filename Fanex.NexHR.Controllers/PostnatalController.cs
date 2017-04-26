namespace Fanex.NexHR.Controllers
{
    using Criteria.Postnatal;
    using Models;
    using Services;
    using Services.Interfaces;
    using ViewModels.Postnatal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService _postnatalService;
        private readonly ICommandService _commandService;

        public PostnatalController()
            : this(null, null)
        {
        }

        public PostnatalController(IPostnatalService postnatalService, ICommandService commandService)
        {
            _postnatalService = postnatalService ?? new PostnatalService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            PostnatalViewModel model = new PostnatalViewModel()
            {
                EmpId = CurrentUser.Id,
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Types = 0,
                Minutes = 30
            };
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(PostnatalViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                InsertPostnatalCriteria criteria = new InsertPostnatalCriteria()
                {
                    EmployeeId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Minutes = model.ChosenTypes.Count == 2 ? 30 : (model.ChosenTypes[0] == 1 ? 60 : 0),
                    Types = model.ChosenTypes.Count == 2 ? 3 : model.ChosenTypes[0]
                };
                CommandResult result = _commandService.Insert(criteria);
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

        public ActionResult Edit(int empId, string from, string to)
        {
            GetByIdPostnatalCriteria criteria = new GetByIdPostnatalCriteria()
            {
                EmployeeId = empId,
                FromDate = DateTime.Parse(from),
                ToDate = DateTime.Parse(to)
            };
            PostnatalViewModel employee = _postnatalService.GetById(criteria);
            if (employee != null)
            {
                employee.OldEmpId = employee.EmpId;
                employee.OldFromDate = employee.FromDate;
                employee.OldToDate = employee.ToDate;
            }
            
            return View(employee);
        }        

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Edit(PostnatalViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                UpdatePostnatalCriteria criteria = new UpdatePostnatalCriteria()
                {
                    OldEmployeeId = model.OldEmpId,
                    OldFromDate = model.OldFromDate,
                    OldToDate = model.OldToDate,
                    NewEmployeeId = model.EmpId,
                    NewFromDate = model.FromDate,
                    NewToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Minutes = model.ChosenTypes.Count == 2 ? 30 : (model.ChosenTypes[0] == 1 ? 60 : 0),
                    Types = model.ChosenTypes.Count == 2 ? 3 : model.ChosenTypes[0]
                };
                CommandResult result = _commandService.Update(criteria);
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

        public JsonResult Delete(int empId, DateTime fromDate, DateTime toDate)
        {
            DeletePostnatalCriteria criteria = new DeletePostnatalCriteria()
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };
            CommandResult result = _commandService.Delete(criteria);
            return Json(result);
        }

        public ActionResult ListPostnatalEmployee(int empId, int year)
        {
            GetAllPostnatalCriteria criteria = new GetAllPostnatalCriteria()
            {
                EmpId = empId,
                Year = year
            };
            List<PostnatalViewModel> listPostnatal = _postnatalService.GetAll(criteria).ToList();

            return PartialView("_ListPostnatalEmployee", listPostnatal);
        }
    }
}
