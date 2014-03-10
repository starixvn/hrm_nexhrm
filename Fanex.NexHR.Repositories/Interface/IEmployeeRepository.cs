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
    using System.Collections;
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

        IList<CheckInOut> GetCheckInOutRequest(ICriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequest(ICriteria criteria);

        IList<ChangingShift> GetChangeShiftRequest(ICriteria criteria);

        IList<Leave> GetLeaveRequest(ICriteria criteria);

        IList<RequestEntity> GetAllRequest(ICriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestByDate(ICriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestByDate(ICriteria criteria);

        IList<ChangingShift> GetChangeShiftRequestByDate(ICriteria criteria);

        IList<Leave> GetLeaveRequestByDate(ICriteria criteria);

        IList<RequestEntity> GetAllRequestByDate(ICriteria criteria);

        IList<CheckInOut> GetCheckInOutRequestHistory(ICriteria criteria);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(ICriteria criteria);

        IList<ChangingShift> GetChangeShiftRequestHistory(ICriteria criteria);

        IList<Leave> GetLeaveRequestHistory(ICriteria criteria);

        IList<Employee> GetMembersByManager(ICriteria criteria);

        bool AdjustAnnualLeaves(ICriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(ICriteria criteria);

        bool DeleteAnnualLeaveTransaction(ICriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionByID(ICriteria criteria);

        bool UpdateAnnualLeaveTransaction(ICriteria criteria);

        bool InsertAnnualLeaveTransaction(ICriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees(ICriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(ICriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(ICriteria criteria);

        Hashtable UpdateOffDayForAllEmployees(ICriteria criteria);

        int GetRangeDate(ICriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(ICriteria criteria);

        bool ResetAnnualLeaves(ICriteria criteria);

        IList<Employee> GetEmployeeTOEICScore(ICriteria criteria);

        void GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria, ref IList<Employee> employeesBirthday, ref IList<Employee> employeesFirstWorkingday);
    }
}
