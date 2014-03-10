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

        IList<ChangingShift> GetChangeShiftRequest(GetChangeShiftRequestCriteria criteria);

        IList<Leave> GetLeaveRequest(GetLeaveRequestCriteria criteria);

        IList<RequestEntity> GetAllRequest(GetAllRequestCriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestByDate(GetCheckInOutRequestByDateCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(GetLateInOutRequestByDateCriteria criteria);

        IList<ChangingShift> GetChangeShiftRequestByDate(GetChangeShiftRequestByDateCriteria criteria);

        IList<Leave> GetLeaveRequestByDate(GetLeaveRequestByDateCriteria criteria);

        IList<RequestEntity> GetAllRequestByDate(GetAllRequestByDateCriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestHistory(GetRequestHistoryCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(GetRequestHistoryCriteria criteria);

        IList<ChangingShift> GetChangeShiftRequestHistory(GetRequestHistoryCriteria criteria);

        IList<Leave> GetLeaveRequestHistory(GetRequestHistoryCriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionByID(GetAnnualLeaveByIdCriteria criteria);

        bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria);

        bool UpdateOffDayForAllEmployees(ModelStateDictionary modelState, UpdateOffDayForAllEmployeesCriteria criteria);

        int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> teams);

        IList<Employee> GetEmployeeTOEICScore(GetEmployeeTOEICScoreCriteria criteria);

        void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday);
    }
}
