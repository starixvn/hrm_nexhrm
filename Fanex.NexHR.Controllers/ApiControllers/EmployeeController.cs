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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Employee controller.
    /// </summary>
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

        /// <summary>
        /// Method get Employees.
        /// </summary>
        /// <param name="query">String to query.</param>
        /// <param name="groupId">Employee group id.</param>
        /// <param name="enable">Is enabled.</param>
        /// <param name="isDelete">Is Deleted.</param>
        /// <param name="titleId">Job title id.</param>
        /// <returns>List of Employee.</returns>
        public IQueryable<Employee> GetEmployees(string query, int? groupId = null, bool? enable = null, bool? isDelete = null, int? titleId = null)
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

            return entityList.AsQueryable();
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            var deleteWorkingTimeRequestCriteria = new DeleteWorkingTimeRequestCriteria() { Id = id };
            bool deleted = _employeeService.DeleteWorkingTimeRequest(deleteWorkingTimeRequestCriteria);
            return deleted;
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

        /// <summary>
        /// Get date range.
        /// </summary>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="fromDate">Get from date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <returns>Total days.</returns>
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

        /// <summary>
        /// Get TOEIC score.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="groupId">The group ID.</param>
        /// <param name="titleId">The title ID.</param>
        /// <param name="score">The score.</param>
        /// <param name="enable">The enable.</param>
        /// <param name="isDelete">The is delete.</param>
        /// <returns>List Employee with TOEIC score.</returns>
        public IList<Employee> GetToeic(string query, int? groupId = null, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null)
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

        /// <summary>
        /// Method check User query condition.
        /// </summary>
        /// <param name="enable">Enable condition.</param>
        /// <returns>A value to query DB.</returns>
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

        /// <summary>
        /// Method check User query condition.
        /// </summary>
        /// <param name="isDelete">Is deleted.</param>
        /// <returns>A value to query DB.</returns>
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