namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeService
    {
        Employee GetEmployeeById(GetEmployeeByIdCriteria criteria);

        Employee GetEmployeeByDomainName(GetEmployeeByDomainNameCriteria criteria);

        IList<Employee> Query(GetEmployeeCriteria criteria);

        CommandResult Insert(InsertEmployeeCriteria criteria);

        CommandResult Delete(DeleteEmployeeCriteria criteria);

        CommandResult Update(UpdateEmployeeCriteria criteria);

        IList<Employee> GetMembersByManager(GetMemberByManagerCriteria criteria);

        DefaultSchedule GetEmployeeShift(GetEmployeeShiftCriteria criteria);

        CommandResult UpdateEmployeeShift(UpdateScheduleDefaultCriteria criteria);

        MySpaceViewModel GetMySpace(GetMySpaceCriteria criteria);

        HRLeaderDashboardModel GetHRLeaderDashboard(GetHRLeaderDashboardCriteria criteria);

        ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance(GetOpenAndCloseAnnualBalanceCriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(GetAnnualLeaveByIdCriteria criteria);

        bool DeleteAnnualLeaveTransaction(DeleteAnnualLeaveTransactionCriteria criteria);

        bool UpdateAnnualLeaveTransaction(UpdateAnnualLeaveTransactionCriteria criteria);

        bool InsertAnnualLeaveTransaction(InsertAnnualLeaveTransactionCriteria criteria);

        CommandResult AddLeaveDeduction(InsertLeaveDeductionCriteria criteria);

        CommandResult EditLeaveDeduction(UpdateLeaveDeductionCriteria criteria);

        CommandResult DeleteLeaveDeduction(DeleteLeaveDeductionCriteria criteria);

        int GetRangeDate(GetDateRangeEmployeeCalendarCriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(GetUnavailableRequestDateCriteria criteria);

        IList<LeaveSummaryByTeam> GetAnnualLeaveByTeam(IList<LeaveSummary> leaveSummaryList, IList<Team> listTeam);

        IList<Employee> GetEmployeeToeicScore(GetEmployeeToeicScoreCriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(GetBirthdayAndFirstWorkingdayCriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(GetUpdatingScoreRelatedPeopleCriteria criteria);

        bool AdjustAnnualLeaves(AdjustAnnualLeavesCriteria criteria);

        bool OpenAnnualLeaves(OpenAnnualBalanceForNewYearCriteria criteria);

        bool ResetAnnualLeaves(ResetAnnualLeavesCriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutByEmpID(GetMissingCheckInOutEmployeesCriteria criteria);
    }
}