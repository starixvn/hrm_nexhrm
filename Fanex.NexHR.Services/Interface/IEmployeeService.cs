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

        IList<Employee> GetEmployeeForRelatedTeam(GetEmployeeForRelatedTeamCriteria criteria);

        IList<Employee> GetLeaders(GetLeadersOfEmployeeCriteria criteria);

        int Insert(InsertEmployeeCriteria criteria);

        bool Delete(DeleteEmployeeCriteria criteria);

        bool Update(UpdateEmployeeCriteria criteria);

        EmployeeShift GetEmployeeShift(GetEmployeeShift criteria);

        bool UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        HrLeaderDashboardModel GetHrLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria);

        WorkingTimeRequest GetWorkingTimeRequest(GetWorkingTimeRequestCriteria criteria);

        bool DeleteWorkingTimeRequest(DeleteWorkingTimeRequestCriteria criteria);

        bool UpdateShift(UpdateShiftCriteria criteria);

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

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(GetAnnualLeaveByIdCriteria criteria);

        bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

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
