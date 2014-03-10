// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee Get(GetEmployeeCriteria criteria);

        IList<Employee> Query(GetAllEmployeeCriteria criteria);

        IList<Employee> GetEmployeeForRelatedTeam(ICriteria criteria);

        IList<Employee> GetEmployeeByTeams(int employeeId, string teamCodes, string ignoreTeams);

        IList<Employee> GetLeaders(ICriteria criteria);

        int Insert(ICriteria criteria);

        bool Delete(ICriteria criteria);

        bool Update(ICriteria criteria);

        EmployeeShift GetEmployeeShift(ICriteria criteria);

        bool UpdateEmployeeShift(ICriteria criteria);

        bool UpdateWeekWorking(EmployeeShift model, DateTime date);

        long WorkingTimeRequest(
            int empId,
            short requestType,
            bool isPlanned,
            string remark,
            DateTime createdDate,
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

        int ProcessRequest(
            Commands command,
            long requestId,
            int processedBy,
            DateTime processedDate,
            out string message);

        bool UpdateRequest(
            long requestId,
            bool isPlanned,
            string remark,
            int modifiedby,
            DateTime fromDate,
            DateTime modifiedDate,
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

        WorkingTimeRequest GetWorkingTimeRequest(long workingTimeId);

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

        IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria);

        bool AdjustAnnualLeaves(AdjustAnnualLeavesCriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(GetAnnualLeaveByIdCriteria criteria);

        bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        bool UpdateOffDayForAllEmployees(DateTime date, int isOffDay, int modifiedBy, DateTime modifiedDate, out int errorCode, out string message);

        int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        bool ResetAnnualLeaves(ResetAnnualLeavesCriteria criteria);

        IList<Employee> GetEmployeeTOEICScore(string query, int? groupId, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null);

        void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday);
    }
}
