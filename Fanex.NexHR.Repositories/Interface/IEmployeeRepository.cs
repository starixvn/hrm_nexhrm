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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee Get(int? employeeId = null, string domainUsername = null);

        IList<Employee> Query(string query, int? groupId, bool? enable = null, bool? isDelete = null, int? titleId = null);

        IList<Employee> GetEmployeeForRelatedTeam(int employeeId);

        IList<Employee> GetEmployeeByTeams(int employeeId, string teamCodes, string ignoreTeams);

        IList<Employee> GetAll();

        IList<Employee> GetLeaders(int id);

        int Insert(CreateEmployeeViewModel model);

        bool Delete(int id, DateTime resignedDate);

        bool Update(Employee model);

        EmployeeShift GetEmployeeShift(int id);

        bool UpdateEmployeeShift(EmployeeShift model);

        IList<WeekWorking> GetWeekWorking(int id, DateTime date);

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

        IList<CheckInOut> GetCheckInOutRequest(
            short roleId,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? logInId = null);

        IList<LateInEarlyOut> GetLateInEarlyOutRequest(
            short roleID,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? longInId = null);

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
            short roleID,
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? groupId = null,
            byte? status = null,
            int? longInId = null);

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

        IList<CheckInOut> GetCheckInOutRequestHistory(
            long requestId,
            int requestTypeId);

        IList<LateInEarlyOut> GetLateInEarlyOutRequestHistory(
            long requestId,
            int requestTypeId);

        IList<ChangingShift> GetChangeShiftRequestHistory(
            long requestId,
            int requestTypeId);

        IList<Leave> GetLeaveRequestHistory(
        long requestId,
        int requestTypeId);

        IList<Employee> GetMembersByManager(int managerId, short role, string keyword, int? groupId = null);

        bool AdjustAnnualLeaves(
            string empIds,
            string description,
            DateTime fromDate,
            decimal deductDays,
            decimal creditDays,
            DateTime createdDate,
            int createdBy);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(int id, int year, bool excludedRequestTrans);

        bool DeleteAnnualLeaveTransaction(long id, DateTime modifiedDate, int modifiedBy);

        AnnualLeaveBalanceModel GetAnnualLeaveTransaction(int id);

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

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees();

        IList<UserChangedShift> GetUserChangedShift(DateTime date);

        IList<HalfDayOff> GetHalfDayOffs(DateTime reportDate, int? employeeId = null, int? groupId = null);

        bool UpdateOffDayForAllEmployees(DateTime date, int isOffDay, int modifiedBy, DateTime modifiedDate, out int errorCode, out string message);

        int GetRangeDate(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DateTime> GetDisableRequestDate(int employeeId, DateTime fromDate, DateTime toDate);

        bool ResetAnnualLeaves(string empValue, string description, DateTime fromDate, int resetDays, DateTime createdDate, int createdBy);

        IList<Employee> GetEmployeeTOEICScore(string query, int? groupId, int? titleId = null, int? score = null, bool? enable = null, bool? isDelete = null);
    }
}
