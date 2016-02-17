namespace Fanex.NexHR.Controllers.ApiControllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController() : this(null) 
        { 
        }

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
        }

        public IQueryable<Employee> GetEmployees(
            string query,
            int? teamId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            if (teamId == -1)
            {
                teamId = null;
            }

            var criteria = new GetEmployeeCriteria()
            {
                Query = query,
                TeamId = teamId,
                JobTitleId = jobTitleId,
                Enable = CheckUserEnableCondition(enable),
                IsDelete = CheckDeletedUserCondition(isDelete),
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            var employees = employeeService.Query(criteria);

            employees = SortEmployee(sort, employees);

            return employees.AsQueryable();
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            var workingScheduleCriteria = new GetWorkingScheduleCriteria
            {
                ReportDate = date,
                EmployeeId = id
            };

            return employeeService.GetWorkingSchedule(workingScheduleCriteria);
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var getDateRangeCriteria = new GetDateRangeEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            int totalDays = employeeService.GetRangeDate(getDateRangeCriteria);

            return totalDays;
        }

        public IEnumerable<DateTime> GetUnavailDate(int employeeId)
        {
            var getUnavailableRequestDateCriteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = employeeId,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            return employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
        }

        public IList<Employee> GetToeic(
            string query,
            int? groupId = null,
            int? titleId = null,
            int? score = null,
            bool? enable = null,
            bool? isDelete = null)
        {
            if (groupId == -1)
            {
                groupId = null;
            }

            var getToeicScoreCriteria = new GetEmployeeToeicScoreCriteria
            {
                Query = query,
                GroupId = groupId,
                TitleId = titleId,
                Score = score,
                Enable = enable,
                IsDelete = isDelete
            };
            IList<Employee> employees = employeeService.GetEmployeeToeicScore(getToeicScoreCriteria);
            return employees;
        }

        private int? CheckUserEnableCondition(bool? enable)
        {
            int? disableUsers;
            if (enable == true)
            {
                disableUsers = 0;
            }
            else if (enable == false)
            {
                disableUsers = 1;
            }
            else
            {
                disableUsers = null;
            }

            return disableUsers;
        }

        private int? CheckDeletedUserCondition(bool? isDelete)
        {
            int? deleteUsers;
            if (isDelete == true)
            {
                deleteUsers = 1;
            }
            else if (isDelete == false)
            {
                deleteUsers = 0;
            }
            else
            {
                deleteUsers = null;
            }

            return deleteUsers;
        }

        private IList<Employee> SortEmployee(string sort, IList<Employee> employees)
        {
            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                employees = Utilities.MultipleSort(employees, sorts);
            }

            return employees;
        }
    }
}