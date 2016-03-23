namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Workload;

    public interface IEmployeeRepository
    {
        Employee GetById(ICriteria criteria);

        Employee GetByDomainName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        IList<DashBoardPersonalSpace> GetDashBoardPersonalSpace(ICriteria criteria);

        IList<DashBoardCompanySpace> GetDashBoardCompanySpace(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        List<Workload> GetWorkloadByTeam(ICriteria criteria);
    }
}