namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Postnatal;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class PostnatalController : BaseController
    {
        private readonly IPostnatalService postnatalService;
        private readonly ICommandService commandService;

        public PostnatalController()
            : this(null, null)
        {
        }

        public PostnatalController(IPostnatalService postnatalService, ICommandService commandService)
        {
            this.postnatalService = postnatalService ?? new PostnatalService();
            this.commandService = commandService ?? new CommandService();
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
                Type = 0,
                Minutes = 30
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PostnatalViewModel model)
        {
            CommandResult result = new CommandResult();
            if (ModelState.IsValid)
            {
                InsertPostnatalCriteria criteria = new InsertPostnatalCriteria()
                {
                    EmployeeId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Minutes = model.Minutes,
                    Type = model.Types.Count == 2 ? 3 : model.Types[0]
                };                
                result = commandService.Insert(criteria);
                if (result.Code == 0)
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
            PostnatalViewModel employee = postnatalService.GetById(criteria);
            if (employee != null)
            {
                employee.OldEmpId = employee.EmpId;
                employee.OldFromDate = employee.FromDate;
                employee.OldToDate = employee.ToDate;
            }
            
            return View(employee);
        }        

        [HttpPost]
        public ActionResult Edit(PostnatalViewModel model)
        {
            CommandResult result = new CommandResult();
            if (ModelState.IsValid)
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
                    Minutes = model.Minutes,
                    Type = model.Types.Count == 2 ? 3 : model.Types[0]
                };
                result = commandService.Update(criteria);
                if (result.Code == 0)
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
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        public void Delete(int empId, DateTime fromDate, DateTime toDate)
        {
            DeletePostnatalCriteria criteria = new DeletePostnatalCriteria()
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };
            commandService.Delete(criteria);
        }

        public ActionResult ListPostnatalEmployee(int empId, int year)
        {
            GetAllPostnatalCriteria criteria = new GetAllPostnatalCriteria()
            {
                EmpId = empId,
                Year = year
            };
            List<PostnatalViewModel> listPostnatal = postnatalService.GetAll(criteria).ToList();

            return PartialView(listPostnatal);
        }
    }
}
