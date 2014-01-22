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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
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

        public IQueryable<Employee> GetEmployees(string query, int? groupId = null, bool? enable = null, bool? isDelete = null, int? titleId = null)
        {
            if (groupId == -1)
            {
                groupId = null;
            }

            var entityList = _employeeService.Query(query, groupId, enable, isDelete, titleId).AsQueryable();

            return entityList;
        }

        [HttpPost]
        public bool DeleteWorkingTimeRequest(int id)
        {
            return _employeeService.DeleteWorkingTimeRequest(id);
        }

        public IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id)
        {
            return _employeeService.GetWorkingSchedule(date, id);
        }

        public int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return _employeeService.GetRangeDate(employeeId, fromDate, toDate);
        }

        public IEnumerable<DateTime> GetUnavailDate(int employeeId, DateTime date)
        {
            return _employeeService.GetDisableRequestDate(employeeId, new DateTime(date.AddMonths(-1).Year, date.AddMonths(-1).Month, 1), new DateTime(date.AddMonths(2).Year, date.AddMonths(2).Month, 1).AddDays(-1));
        }

        public IList<Employee> GetTOEIC(string query, int? groupId = null, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null)
        {
            if (groupId == -1)
            {
                groupId = null;
            }

            IList<Employee> employees = _employeeService.GetEmployeeTOEICScore(query, groupId, titleId, score, enable, isDelete);
            return employees;
        }
    }
}
