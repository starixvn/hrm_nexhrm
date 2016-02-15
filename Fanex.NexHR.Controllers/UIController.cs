// --------------------------------------------------------------------------
// <copyright file="UIController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

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
            SystemReminderModel lstEmployee = null;
            List<Employee> employeesBirthday = null;
            List<Employee> employeesFirstWorkingday = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            lstEmployee = _employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
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

            return PartialView(this.CurrentUser);
        }

        [ChildActionOnly]
        public ActionResult GlobalScript()
        {
            ViewBag.Employee = this.CurrentUser;
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
            var navigationItemList = new List<NavigationViewModel>();

            var roles = Roles.GetRolesForUser();
            var team = this.CurrentUser.Team.Code;
            navigationItemList = new List<NavigationViewModel>();

            // populate sitemap data
            SiteMapNode root = SiteMap.RootNode;

            foreach (SiteMapNode sectionNode in root.ChildNodes)
            {
                // section level
                if (sectionNode.Roles.Count == 0
                    || this.CurrentUser.IsInRoles(sectionNode.Roles.Cast<string>().ToList<string>()))
                {
                    var sectionItem = new NavigationViewModel(sectionNode);
                    navigationItemList.Add(sectionItem);

                    // populate navigation item by section item
                    sectionItem.Childs = new List<NavigationViewModel>();
                    foreach (SiteMapNode navNode in sectionNode.ChildNodes)
                    {
                        var navItem = new NavigationViewModel(navNode);
                        if (navNode.Roles == null || navNode.Roles.Count == 0 ||
                            roles.Any(i => navNode.Roles.Contains(i)) ||
                            team == navItem.Team)
                        {
                            sectionItem.Childs.Add(navItem);
                        }
                    }
                }
            }

            return navigationItemList;
        }
    }
}