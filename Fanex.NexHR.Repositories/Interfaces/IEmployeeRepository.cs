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

        IList<AnnualLeaveBalanceModel> GetAnnualLeaveTransactions(ICriteria criteria);

        bool DeleteAnnualLeaveTransaction(ICriteria criteria);

        AnnualLeaveBalanceModel GetAnnualLeaveTransactionById(ICriteria criteria);

        bool UpdateAnnualLeaveTransaction(ICriteria criteria);

        bool InsertAnnualLeaveTransaction(ICriteria criteria);

        IList<MissingCheckInOutModel> GetMissingCheckInOutEmployees(ICriteria criteria);

        int GetRangeDate(ICriteria criteria);

        IEnumerable<DateTime> GetDisableRequestDate(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        IList<Employee> GetUpdatingScoreRelatedPeople(ICriteria criteria);
    }
}