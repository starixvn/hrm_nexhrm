namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using Services.Interfaces;
    using ViewModels.Navigation;

    [Authorize]
    public class UIController : BaseController
    {
        private readonly IEmployeeService employeeService;

        public UIController(ISessionService sessionService, IEmployeeService employeeService) : base(sessionService)
        {
            this.employeeService = employeeService;
        }

        public async Task<ActionResult> Header()
        {
            List<Employee> employeesBirthday = null;
            List<Employee> employeesFirstWorkingday = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = DateTime.Now.Date
            };

            var lstEmployee = await employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria).ConfigureAwait(false);
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

        public ActionResult GlobalScript()
        {
            ViewBag.Employee = CurrentUser;
            return PartialView();
        }

        public PartialViewResult NavBar()
        {
            //var routeData = ControllerContext.ParentActionViewContext.RouteData;
            //ViewBag.Controller = routeData.Values["controller"];
            //ViewBag.Action = routeData.Values["action"];
            //ViewBag.NavigationItems = GetNagivationData();

            return PartialView();
        }

        //[NonAction]
        //public IList<NavigationViewModel> GetNagivationData()
        //{
        //    var roles = Roles.GetRolesForUser();
        //    var navigationItemList = new List<NavigationViewModel>();

        //    SiteMapNode root = SiteMap.RootNode;

        //    foreach (SiteMapNode sectionNode in root.ChildNodes)
        //    {
        //        if (!Employee.IsInRoles(sectionNode.Roles.Cast<string>().ToList()))
        //        {
        //            continue;
        //        }

        //        NavigationViewModel sectionItem = CreateSectionItemFromSectionNodeWithRoles(roles, sectionNode);

        //        navigationItemList.Add(sectionItem);
        //    }

        //    return navigationItemList;
        //}

        //private static NavigationViewModel CreateSectionItemFromSectionNodeWithRoles(string[] roles, SiteMapNode sectionNode)
        //{
        //    var sectionItem = new NavigationViewModel(sectionNode)
        //    {
        //        Childs = new List<NavigationViewModel>()
        //    };

        //    foreach (SiteMapNode navNode in sectionNode.ChildNodes)
        //    {
        //        var navItem = new NavigationViewModel(navNode);
        //        if (roles.Any(navNode.Roles.Contains))
        //        {
        //            sectionItem.Childs.Add(navItem);
        //        }
        //    }

        //    return sectionItem;
        //}
    }
}