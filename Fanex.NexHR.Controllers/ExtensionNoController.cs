namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Criteria.ExtensionNo;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Interfaces;
    using ViewModels.ExtensionNo;

    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNoService extensionNoService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public ExtensionNoController(ISessionService sessionService,
            IExtensionNoService extensionNoService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.extensionNoService = extensionNoService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        public async Task<IActionResult> Index()
        {
            GetAllExtensionNoCriteria criteria = new GetAllExtensionNoCriteria();
            IEnumerable<ExtensionNoModel> lstEntity = await extensionNoService.GetAllExtensionNo(criteria);
            var model = (from item in lstEntity.ToList()
                         group item by item.TeamName into g
                         select new ExtensionNoViewModel { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult AddNewMember()
        {
            return View(new ExtensionNoModel());
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> AddNewMember(ExtensionNoModel model)
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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditExtensionNumber(bool isInternal, int empExtId)
        {
            var criteria = new GetEmployeeExtensionNoByIdCriteria
            {
                IsInternal = isInternal,
                EmpExtId = empExtId
            };
            var model = await extensionNoService.GetEmployeeExtensionNoById(criteria);
            var groupCriteria = new GetAllExtensionGroupInfoCriteria
            {
                IsInternal = isInternal,
                GroupId = null
            };
            var group = await extensionNoService.GetAllExtensionGroupInfo(groupCriteria);
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
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> EditExtensionNumber(ExtensionNoModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                CheckExternalEmployee(model);
                var criteria = new GetAllExtensionGroupInfoCriteria
                {
                    IsInternal = model.IsInternal,
                    GroupId = null
                };
                var group = await extensionNoService.GetAllExtensionGroupInfo(criteria);
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(updateCriteria);
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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteExtensionNumber(int extId)
        {
            var criteria = new DeleteExtensionNumberCriteria
            {
                ExtId = extId
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AddNewExternalGroup(string groupName)
        {
            var criteria = new AddNewExternalGroupCriteria
            {
                FullName = WebUtility.HtmlEncode(groupName)
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteExternalGroup(int extId)
        {
            var criteria = new DeleteExternalGroupCriteria
            {
                ExtId = extId
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> EditExternalGroup(int extId, string groupName)
        {
            var criteria = new UpdateExternalGroupCriteria
            {
                ExtId = extId,
                FullName = groupName
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        public async Task<IActionResult> ManageExternalGroup()
        {
            var criteria = new GetAllExtensionGroupInfoCriteria
            {
                IsInternal = false,
                GroupId = null
            };
            var group = await extensionNoService.GetAllExtensionGroupInfo(criteria);

            return View(group);
        }

        public async Task<JsonResult> GetGroupExtension(bool isInternal, int? groupId)
        {
            List<SelectListItem> groupExtensions;
            if (isInternal)
            {
                IList<Team> teams = await teamService.GetTeam();
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
                IEnumerable<ExtensionNoModel> extensions = await extensionNoService.GetAllExtensionGroupInfo(criteria);
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
                foreach (var modelError in ModelState.Select(x => x.Key).Where(y => y.Contains("EmpId")))
                {
                    ModelState[modelError].Errors.Clear();
                }
            }
        }
    }
}