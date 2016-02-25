namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNoService extensionNoService;
        private readonly ICommandService commandService;

        public ExtensionNoController()
            : this(null, null)
        {
        }

        public ExtensionNoController(
            IExtensionNoService extensionNoService,
            ICommandService commandService)
        {
            this.extensionNoService = extensionNoService ?? new ExtensionNoService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            GetAllExtensionNoCriteria criteria = new GetAllExtensionNoCriteria();
            List<ExtensionNoModel> lstEntity = extensionNoService.GetAllExtensionNo(criteria).ToList();
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNoViewModel() { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View(new ExtensionNoModel());
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            if (ModelState.IsValid)
            {
                var criteria = new AddNewExtensionNoMemberCriteria
                {
                    EmpId = model.EmpId,
                    NickName = model.NickName,
                    FullName = model.FullName,
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
                };

                CommandResult result = commandService.Insert(criteria);                
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

        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(bool isInternal, int empExtId)
        {
            var criteria = new GetEmployeeExtensionNoByIdCriteria
            {
                IsInternal = isInternal,
                EmpExtId = empExtId
            };
            var model = extensionNoService.GetEmployeeExtensionNoById(criteria);
            var groupCriteria = new GetAllExtensionGroupInfoCriteria 
            { 
                IsInternal = isInternal,
                GroupId = null
            };
            var group = extensionNoService.GetAllExtensionGroupInfo(groupCriteria);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(ExtensionNoModel model)
        {
            CheckExternalEmployee(model);
            var criteria = new GetAllExtensionGroupInfoCriteria
            {
                IsInternal = model.IsInternal,
                GroupId = null
            };
            var group = extensionNoService.GetAllExtensionGroupInfo(criteria);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            if (ModelState.IsValid)
            {
                var updateCriteria = new UpdateExtensionNoMemberCriteria
                {
                    ExtId = model.EmpExtId,
                    EmpId = model.EmpId,
                    NickName = model.NickName,
                    FullName = model.FullName,
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
                };

                CommandResult result = commandService.Update(updateCriteria);                
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

        public ActionResult ManageExternalGroup()
        {
            var criteria = new GetAllExtensionGroupInfoCriteria
            {
                IsInternal = false,
                GroupId = null
            };
            var group = extensionNoService.GetAllExtensionGroupInfo(criteria);

            return View(group);
        }

        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (model.IsInternal == false)
            {
                foreach (var modelError in ModelState)
                {
                    string propertyName = modelError.Key;

                    if (propertyName.Contains("EmpId"))
                    {
                        ModelState[propertyName].Errors.Clear();
                    }
                }
            }
        }
    }
}
