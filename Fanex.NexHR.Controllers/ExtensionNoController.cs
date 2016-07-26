namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ExtensionNo;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNoService extensionNoService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public ExtensionNoController()
            : this(null, null, null)
        {
        }

        public ExtensionNoController(
            IExtensionNoService extensionNoService,
            ITeamService teamService,
            ICommandService commandService)
        {
            this.extensionNoService = extensionNoService ?? new ExtensionNoService();
            this.teamService = teamService ?? new TeamService();
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

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                CheckExternalEmployee(model);
                var criteria = new AddNewExtensionNoMemberCriteria
                {
                    EmpId = model.EmpId,
                    NickName = HttpUtility.HtmlEncode(model.NickName),
                    FullName = HttpUtility.HtmlEncode(model.FullName),
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
                };

                CommandResult result = commandService.Insert(criteria);
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
            model.FullName = HttpUtility.HtmlDecode(model.FullName);
            model.NickName = HttpUtility.HtmlDecode(model.NickName);

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(ExtensionNoModel model)
        {
            if (model != null && ModelState.IsValid)
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

                var updateCriteria = new UpdateExtensionNoMemberCriteria
                {
                    ExtId = model.EmpExtId,
                    EmpId = model.EmpId,
                    NickName = HttpUtility.HtmlEncode(model.NickName),
                    FullName = HttpUtility.HtmlEncode(model.FullName),
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
                };

                CommandResult result = commandService.Update(updateCriteria);
                if (result.IsSuccessful)
                {
                        ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = HttpUtility.HtmlEncode(groupName)
            };
            CommandResult result = commandService.Insert(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.Delete(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtId = extId,
                FullName = groupName
            };
            CommandResult result = commandService.Update(criteria);

            return Json(result);
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

        public JsonResult GetGroupExtension(bool isInternal, int? groupId)
        {
            var groupExtensions = new List<SelectListItem>();
            if (isInternal)
            {
                IList<Team> teams = teamService.Get();
                groupExtensions = teams.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            }
            else
            {
                var criteria = new GetAllExtensionGroupInfoCriteria
                {
                    IsInternal = isInternal,
                    GroupId = groupId
                };
                IList<ExtensionNoModel> extensions = extensionNoService.GetAllExtensionGroupInfo(criteria);
                groupExtensions = extensions.Select(x => new SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
            }

            return Json(groupExtensions);
        }

        private void CheckExternalEmployee(ExtensionNoModel model)
        {
            if (!model.IsInternal)
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
