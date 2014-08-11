// --------------------------------------------------------------------------
// <copyright file="ExtensionNoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// ExtensionNo Controller.
    /// </summary>
    public class ExtensionNoController : BaseController
    {
        private readonly IExtensionNoService _extensionNoService;

        public ExtensionNoController(IExtensionNoService extensionNoService = null)
        {
            this._extensionNoService = extensionNoService ?? new ExtensionNoService();
        }

        public ExtensionNoController()
            : this(null)
        {
        }

        /// <summary>
        /// Extension Number Index.
        /// </summary>
        /// <returns>View ExtensionNo.</returns>
        public ActionResult Index()
        {
            var criteria = new GetAllExtensionNoCriteria();
            var lstEntity = _extensionNoService.GetAllExtensionNo(criteria);
            var model = (from item in lstEntity
                         group item by item.TeamName into g
                         select new ExtensionNoViewModel() { Name = g.Key, ListExt = g.ToList() }).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult AddNewMember()
        {
            return View();
        }

        /// <summary>
        /// Function to add new employee extension.
        /// </summary>
        /// <param name="model">ExtensionNo Model.</param>
        /// <returns>Action Result.</returns>
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
                ViewBag.Requested = _extensionNoService.AddNewExtensionNoMember(criteria);
            }

            return View();
        }

        /// <summary>
        /// ActionResult EditExtensionNumber.
        /// </summary>
        /// <param name="isInternal">Is internal.</param>
        /// <param name="empExtID">Employee extension ID.</param>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult EditExtensionNumber(bool isInternal, int empExtID)
        {
            var criteria = new GetEmployeeExtensionNoByIdCriteria
            {
                IsInternal = isInternal,
                EmpExtId = empExtID
            };
            var model = _extensionNoService.GetEmployeeExtensionNoById(criteria);
            var groupCriteria = new GetAllExtensionGroupInfoCriteria 
            { 
                IsInternal = isInternal,
                GroupId = null
            };
            var group = _extensionNoService.GetAllExtensionGroupInfo(groupCriteria);
            ViewBag.Group = group.Select(x => new SelectListItem
            {
                Text = x.TeamName,
                Value = x.TeamId.ToString()
            }).ToList();
            return View(model);
        }

        /// <summary>
        /// ActionResult EditExtensionNumber.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
        /// <returns>View Index when edit successful.</returns>
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
            var group = _extensionNoService.GetAllExtensionGroupInfo(criteria);
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

                ViewBag.Updated = _extensionNoService.UpdateExtensionNoMember(updateCriteria);
            }

            return View();
        }

        public ActionResult ManageExternalGroup()
        {
            var criteria = new GetAllExtensionGroupInfoCriteria
            {
                IsInternal = false,
                GroupId = null
            };
            var group = _extensionNoService.GetAllExtensionGroupInfo(criteria);

            return View(group);
        }

        /// <summary>
        /// Method Check External Employee.
        /// </summary>
        /// <param name="model">ExtensionNoModel model.</param>
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
