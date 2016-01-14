// --------------------------------------------------------------------------
// <copyright file="EmployeeController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Newtonsoft.Json;

    public class EmployeeController : ApiController
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController()
            : this(null)
        {
        }

        public EmployeeController(IEmployeeService employeeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
        }

        public IQueryable<Employee> GetEmployees(
            string query,
            int? groupId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            string sort = null)
        {
            if (groupId == -1)
            {
                groupId = null;
            }

            var getAllEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupId = groupId,
                TitleId = titleId,
                Enable = CheckUserEnableCondition(enable),
                IsDelete = CheckDeletedUserCondition(isDelete),
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            var entityList = _employeeService.Query(getAllEmployeeCriteria);

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                entityList = Utilities.MultipleSort(entityList, sorts);
            }

            return entityList.AsQueryable();
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            var workingScheduleCriteria = new GetWorkingScheduleCriteria
            {
                ReportDate = date,
                EmployeeId = id
            };

            return _employeeService.GetWorkingSchedule(workingScheduleCriteria);
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var getDateRangeCriteria = new GetDateRangeEmployeeCalendarCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            int totalDays = _employeeService.GetRangeDate(getDateRangeCriteria);

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

            return _employeeService.GetDisableRequestDate(getUnavailableRequestDateCriteria);
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
            IList<Employee> employees = _employeeService.GetEmployeeToeicScore(getToeicScoreCriteria);
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
    }
}