namespace Fanex.NexHR.Controllers
{
    using Criteria.ExtensionNo;
    using Models;
    using Services;
    using Services.Interfaces;
    using ViewModels.ExtensionNo;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.NexHR.Models.Attributes;
    using System.Net;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNoService extensionNoService;
        private readonly ITeamService teamService;
        private readonly ICommandService commandService;

        public ExtensionNoController(ISessionService sessionService,
            IExtensionNoService extensionNoService,
            ITeamService teamService,
            ICommandService commandService) : base(sessionService)
        {
            this.extensionNoService = extensionNoService;
            this.teamService = teamService;
            this.commandService = commandService;
        }

        public ActionResult Index()
        {
            GetAllExtensionNoCriteria criteria = new GetAllExtensionNoCriteria();
            List<ExtensionNoModel> lstEntity = extensionNoService.GetAllExtensionNo(criteria).ToList();
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNoViewModel { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View(new ExtensionNoModel());
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public ActionResult AddNewMember(ExtensionNoModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                CheckExternalEmployee(model);
                var criteria = new AddNewExtensionNoMemberCriteria
                {
                    EmpId = model.EmpId,
                    NickName = WebUtility.HtmlEncode(model.NickName),
                    FullName = WebUtility.HtmlEncode(model.FullName),
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
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

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
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
            model.FullName = WebUtility.HtmlDecode(model.FullName);
            model.NickName = WebUtility.HtmlDecode(model.NickName);

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
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
                    NickName = WebUtility.HtmlEncode(model.NickName),
                    FullName = WebUtility.HtmlEncode(model.FullName),
                    GroupId = model.TeamId,
                    IsInternal = model.IsInternal,
                    ExtensionNo = model.ExtensionNo
                };

                CommandResult result = commandService.ExecuteNonQuery(updateCriteria);
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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = WebUtility.HtmlEncode(groupName)
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtId = extId
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtId = extId,
                FullName = groupName
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);

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
            List<SelectListItem> groupExtensions;
            if (isInternal)
            {
                IList<Team> teams = teamService.GetTeam();
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
                    IsInternal = false,
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
