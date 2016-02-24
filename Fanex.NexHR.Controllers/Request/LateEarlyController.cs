namespace Fanex.NexHR.Controllers.Request
{
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [Authorize]
    public class LateEarlyController : BaseController
    {
        private readonly ILateInEarlyOutService lateinEarlyoutService;

        public LateEarlyController()
            : this(null)
        {
        }

        public LateEarlyController(ILateInEarlyOutService service)
        {
            this.lateinEarlyoutService = service ?? new LateInEarlyOutService();
        }

        public ActionResult Index(int month = 0, int year = 0)
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
            if (ModelState.IsValid)
            {
                InsertLateInEarlyOutCriteria criteria = new InsertLateInEarlyOutCriteria()
                {
                    EmpId = model.EmpId,
                    Type = model.TypeCode,
                    Minutes = model.Minutes,
                    WorkDate = model.WorkDate,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
                CommandResult result = lateinEarlyoutService.Insert(criteria);              
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

        public ActionResult Edit(int id)
        {
            LateInEarlyOutViewModel model = new LateInEarlyOutViewModel();
            GetLateInEarlyOutByIdCriteria criteria = new GetLateInEarlyOutByIdCriteria()
            {
                Id = id
            };

            if (id > 0)
            {
                model = lateinEarlyoutService.GetById(criteria);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(LateInEarlyOutViewModel model)
        {
            if (model.EmpId <= 0)
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidEmployeeID);
            }
            else
            {
                UpdateLateInEarlyOutCriteria criteria = new UpdateLateInEarlyOutCriteria()
                {
                    Id = (int)model.Id,
                    EmpId = model.EmpId,
                    Type = int.Parse(model.Type),
                    Minutes = model.Minutes,
                    WorkDate = model.WorkDate,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id,
                };
                CommandResult result = lateinEarlyoutService.Update(criteria);                
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

        public void Delete(int id)
        {
            DeleteLateInEarlyOutCriteria criteria = new DeleteLateInEarlyOutCriteria() { Id = id };
            if (criteria.IsValid())
            {
                lateinEarlyoutService.Delete(criteria);
            }
        }

        public ActionResult ListLateEarly(int empId, int year)
        {
            return PartialView(GetLateEarlyByEmpId(empId, year));
        }

        private IList<LateInEarlyOutViewModel> GetLateEarlyByEmpId(int empId, int year)
        {
            var criteria = new GetLateInEarlyOutByEmpIdCriteria()
            {
                EmpId = empId,
                Year = year
            };
            IList<LateInEarlyOutViewModel> list = lateinEarlyoutService.GetAllByEmpId(criteria).ToList();

            return list;
        }
    }
}
