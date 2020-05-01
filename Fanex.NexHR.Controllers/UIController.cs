// --------------------------------------------------------------------------
// <copyright file="UIController.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Criteria;
    using Models;
    using Services;
    using Services.Interfaces;
    using ViewModels.Navigation;

    /// <summary>
    /// UI controller.
    /// </summary>
    [Authorize]
    public class UIController : BaseController
    {
        private readonly IEmployeeService _employeeService;

        public UIController()
        {
            _employeeService = new EmployeeService();
        }

        /// <summary>
        /// Action to render header.
        /// </summary>
        /// <returns>Action Result.</returns>
        [ChildActionOnly]
        public ActionResult Header()
        {
            List<Employee> employeesBirthday = null;
            List<Employee> employeesFirstWorkingday = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            var lstEmployee = _employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
            if (lstEmployee != null)
            {
                employeesBirthday = lstEmployee.EmployeeHasBirthday;
                employeesFirstWorkingday = lstEmployee.EmployeeHasFirstWorkingDay;
            }

            var hasBirthdayOrFirstWorkingday = (employeesBirthday != null && employeesBirthday.Any())
                || (employeesFirstWorkingday != null && employeesFirstWorkingday.Any());

            ViewBag.HasBirthdayOrFirstWorkingday = hasBirthdayOrFirstWorkingday;
            ViewBag.EmployeesBirthday = employeesBirthday;
            ViewBag.EmployeesFirstWorkingday = employeesFirstWorkingday;

            return PartialView(CurrentUser);
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = CurrentUser;
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult NavBar()
        {
            var routeData = ControllerContext.ParentActionViewContext.RouteData;
            ViewBag.Controller = routeData.Values["controller"];
            ViewBag.Action = routeData.Values["action"];
            ViewBag.NavigationItems = GetNagivationData();

            return PartialView();
        }

        /// <summary>
        /// Get navigation site map.
        /// </summary>
        /// <returns>Return list.</returns>
        [NonAction]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Reviewed. Suppression is OK here.")]
        public IList<NavigationViewModel> GetNagivationData()
        {
            var roles = Roles.GetRolesForUser();
            var navigationItemList = new List<NavigationViewModel>();

            SiteMapNode root = SiteMap.RootNode;

            foreach (SiteMapNode sectionNode in root.ChildNodes)
            {
                if (!Employee.IsInRoles(sectionNode.Roles.Cast<string>().ToList()))
                {
                    continue;
                }

                NavigationViewModel sectionItem = CreateSectionItemFromSectionNodeWithRoles(roles, sectionNode);

                navigationItemList.Add(sectionItem);
            }

            return navigationItemList;
        }

        private static NavigationViewModel CreateSectionItemFromSectionNodeWithRoles(string[] roles, SiteMapNode sectionNode)
        {
            var sectionItem = new NavigationViewModel(sectionNode)
            {
                Childs = new List<NavigationViewModel>()
            };

            foreach (SiteMapNode navNode in sectionNode.ChildNodes)
            {
                var navItem = new NavigationViewModel(navNode);
                if (roles.Any(navNode.Roles.Contains))
                {
                    sectionItem.Childs.Add(navItem);
                }
            }

            return sectionItem;
        }
    }
}