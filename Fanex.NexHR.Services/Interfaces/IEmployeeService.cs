// --------------------------------------------------------------------------
// <copyright file="IEmployeeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeService
    {
        Employee GetEmployeeById(GetEmployeeCriteria criteria);

        IList<Employee> Query(GetAllEmployeeCriteria criteria);

        IList<Employee> GetEmployeeForRelatedTeam(GetEmployeeForRelatedTeamCriteria criteria);

        IList<Employee> GetLeaders(GetLeadersOfEmployeeCriteria criteria);

        IList<Employee> GetManagements(GetManagementsCriteria criteria);

        int Insert(InsertEmployeeCriteria criteria);

        bool Delete(DeleteEmployeeCriteria criteria);

        bool Update(UpdateEmployeeCriteria criteria);

        EmployeeShift GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        bool UpdateEmployeeShift(UpdateEmployeeShiftCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(GetWorkingCalendarCriteria criteria);

        WorkingTimeRequest GetWorkingTimeRequest(GetWorkingTimeRequestCriteria criteria);

        bool DeleteWorkingTimeRequest(DeleteWorkingTimeRequestCriteria criteria);

        bool UpdateShift(UpdateShiftCriteria criteria);

        IList<CheckInOut> GetCheckInOutRequest(GetCheckInOutRequestCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequest(GetLateInOutRequestCriteria criteria);

        IList<Leave> GetLeaveRequest(GetLeaveRequestCriteria criteria);

        IList<RequestEntity> GetAllRequest(GetAllRequestCriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestByDate(GetCheckInOutRequestByDateCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(GetLateInOutRequestByDateCriteria criteria);

        IList<Leave> GetLeaveRequestByDate(GetLeaveRequestByDateCriteria criteria);

        IList<RequestEntity> GetAllRequestByDate(GetAllRequestByDateCriteria criteria);

        bool InsertWorkingCalendar(InsertEmployeeCalendarCriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestHistory(GetRequestHistoryCriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(GetRequestHistoryCriteria criteria);

        IList<Leave> GetLeaveRequestHistory(GetRequestHistoryCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(GetUserChangedShiftCriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(GetHalfDayOffCriteria criteria);

        bool UpdateOffDayForAllEmployees(ModelStateDictionary modelState, UpdateOffDayForAllEmployeesCriteria criteria);

        int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> listTeam);

        IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);
    }
}
