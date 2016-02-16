// --------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>
namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface IEmployeeRepository
    {
        Employee GetById(ICriteria criteria);

        Employee GetByDomainName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        MySpaceViewModel GetMySpace(ICriteria criteria);

        IList<WorkingScheduleModel> GetWorkingSchedule(GetWorkingScheduleCriteria criteria);

        IList<WorkingCalendarsViewModel> GetWorkingCalendars(ICriteria criteria);

        bool UpdateShift(ICriteria criteria);

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(ICriteria criteria);

        bool DeleteAnnualLeaveTransaction(ICriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(ICriteria criteria);

        bool UpdateAnnualLeaveTransaction(ICriteria criteria);

        bool InsertAnnualLeaveTransaction(ICriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees(ICriteria criteria);

        IList<UserChangedShift> GetUserChangedShift(ICriteria criteria);

        IList<HalfDayOff> GetHalfDayOffs(ICriteria criteria);

        int GetRangeDate(ICriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(ICriteria criteria);
    }
}