namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models;
    using Models.Workload;

    public interface IEmployeeRepository
    {
        Employee GetById(ICriteria criteria);

        Employee GetByDomainName(ICriteria criteria);

        Employee GetByNickName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        IList<DashBoardPersonalSpace> GetDashBoardPersonalSpace(ICriteria criteria);

        IList<DashBoardCompanySpace> GetDashBoardCompanySpace(ICriteria criteria);

        IList<Employee> GetEmployeeToeicScore(ICriteria criteria);

        SystemReminderModel GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        List<Workload> GetWorkloadByTeam(ICriteria criteria);
    }
}