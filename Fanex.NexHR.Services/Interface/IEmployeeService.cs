// --------------------------------------------------------------------------
// <copyright file="IEmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeService
    {
        Employee Get(GetEmployeeCriteria criteria);

        IList<Employee> Query(GetAllEmployeeCriteria criteria);

        IList<Employee> GetEmployeeForRelatedTeam(ICriteria criteria);

        IList<Employee> GetLeaders(ICriteria criteria);

        int Insert(ICriteria criteria);

        bool Delete(ICriteria criteria);

        bool Update(ICriteria criteria);

        EmployeeShift GetEmployeeShift(ICriteria criteria);

        bool UpdateEmployeeShift(ICriteria criteria);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            int createdBy,
            out int errorCode,
            out string message,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null);

        int ApproveRequest(
            long requestId,
            int approvedBy,
            DateTime approvedDate,
            int modifiedBy,
            out string message);

        int DisapproveRequest(long requestId, int modifiedBy, DateTime approvedDate, out string message);

        int RejectRequest(
            long requestId,
            int processedBy,
            DateTime processedDate,
            int modifiedBy,
            out string message);

        bool UpdateRequest(
            long requestId,
            bool isPlanned,
            string remark,
            int modifiedby,
            DateTime fromDate,
            int? fromShift = null,
            DateTime? toDate = null,
            int? toShift = null,
            int? leaveTypeId = null,
            decimal? totalDays = null,
            int? lateInMins = null,
            int? earlyOutMins = null);

        MySpaceViewModel GetMySpace(int id);

        HrLeaderDashboardModel GetHrLeaderDashboard(DateTime date, int? employeeId = null);

        IList<WorkingScheduleModel> GetWorkingSchedule(DateTime date, int id);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(string date);

        IList<WorkingTimeRequest> GetWorkingTimeRequests(int id, int? requestType = null, byte? status = null);

        WorkingTimeRequest GetWorkingTimeRequest(long id);

        bool DeleteWorkingTimeRequest(int id);

        bool UpdateShift(int id, DateTime date, int? shift, int modifiedBy, DateTime modifiedDate);

        IList<CheckInOut> GetCheckInOutRequest(GetCheckInOutRequestCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequest(GetLateInOutRequestCriteria criteria);

        IList<ChangingShift> GetChangeShiftRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<Leave> GetLeaveRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetAllRequest(
           short roleId,
           DateTime fromDate,
           DateTime toDate,
           int? employeeId = null,
           int? groupId = null,
           byte? status = null,
           int? logInId = null);

        IList<CheckInOut> GetCheckInOutRequestByDate(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<ChangingShift> GetChangeShiftRequestByDate(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<Leave> GetLeaveRequestByDate(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<RequestEntity> GetAllRequestByDate(
           short roleId,
           DateTime fromDate,
           DateTime toDate,
           int? employeeId = null,
           int? groupId = null,
           byte? status = null,
           int? logInId = null);

        IList<CheckInOut> GetCheckInOutRequestHistory(long requestId, int requestTypeId);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(long requestId, int requestTypeId);

        IList<ChangingShift> GetChangeShiftRequestHistory(long requestId, int requestTypeId);

        IList<Leave> GetLeaveRequestHistory(long requestId, int requestTypeId);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

        bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy);

        bool UpdateAnnualLeaveTransaction(
            long id,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime modifiedDate,
            int modifiedBy);

        bool InsertAnnualLeaveTransaction(
            int empId,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy);

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        bool UpdateOffDayForAllEmployees(ModelStateDictionary modelState, ChangeWorkingCalendarsViewModel model);

        int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams);

        IList<Employee> GetEmployeeTOEICScore(string query, int? groupId, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null);

        void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday);
    }
}
