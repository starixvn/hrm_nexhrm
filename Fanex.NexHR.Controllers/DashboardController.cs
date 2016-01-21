namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IEmployeeService _employeeService;


        public DashboardController() : this(null)
        {
        }

        public DashboardController(
            IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }


        public ActionResult Index()
        {
            var getMySpaceCriteria = new GetMySpaceCriteria()
            {
                EmpId = this.CurrentUser.Id
            };
            MySpaceViewModel model = _employeeService.GetMySpace(getMySpaceCriteria);

            return View(model);
        }

        private void Celebrate()
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
        }
    }
}
