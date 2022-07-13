namespace Fanex.NexHR.WebSite.Views.Shared.Components.Header
{
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HeaderViewComponent : ViewComponent
    {
        private readonly IEmployeeService employeeService;
        private readonly ISessionService sessionService;

        public HeaderViewComponent(IEmployeeService employeeService, ISessionService sessionService)
        {
            this.employeeService = employeeService;
            this.sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Employee> employeesBirthday = new List<Employee>();
            List<Employee> employeesFirstWorkingday = new List<Employee>();
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

            return View(sessionService.CurrentEmployee);
        }
    }
}