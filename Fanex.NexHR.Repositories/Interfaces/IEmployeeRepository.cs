namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface IEmployeeRepository
    {
        Employee GetByDomainName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        IList<DashBoardPersonalSpace> GetDashBoardPersonalSpace(ICriteria criteria);

        IList<DashBoardCompanySpace> GetDashBoardCompanySpace(ICriteria criteria);

        int GetRangeDate(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);
    }
}