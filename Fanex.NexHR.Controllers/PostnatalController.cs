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
            return View(new PostnatalViewModel());
        }

        [HttpPost]
        public CommandResult Create(PostnatalViewModel model)
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
                    Minutes = model.Minutes
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
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return result;
        }

        public ActionResult Edit(int empId, DateTime fromDate, DateTime toDate)
        {
            GetByIdPostnatalCriteria criteria = new GetByIdPostnatalCriteria()
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };
            PostnatalViewModel employee = postnatalService.GetById(criteria);
            return View(employee);
        }

        [HttpPost]
        public CommandResult Edit(Postnatal model)
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
                    Minutes = model.Minutes
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

            return result;
        }

        public void Delete(int empId, DateTime toDate, DateTime fromDate)
        {
            DeletePostnatalCriteria criteria = new DeletePostnatalCriteria()
            {
                EmployeeId = empId,
                FromDate = fromDate,
                ToDate = toDate
            };
            commandService.Delete(criteria);
        }

        public IList<PostnatalViewModel> ListPostnatalEmployee()
        {
            GetAllPostnatalCriteria criteria = new GetAllPostnatalCriteria();
            List<PostnatalViewModel> listPostnatal = postnatalService.GetAll(criteria).ToList();

            return listPostnatal;
        }
    }
}
