namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models;
    using Models.Workload;

    public interface IEmployeeRepository
    {
        Employee GetById(ICriteria criteria);

        Employee GetByDomainName(ICriteria criteria);

        Employee GetByNickName(ICriteria criteria);

        IList<Employee> Query(ICriteria criteria);

        Task<IList<DashBoardPersonalSpace>> GetDashBoardPersonalSpace(ICriteria criteria);

        Task<IList<DashBoardCompanySpace>> GetDashBoardCompanySpace(ICriteria criteria);

        Task<IList<Employee>> GetEmployeeToeicScore(ICriteria criteria);

        Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        Task<List<Workload>> GetWorkloadByTeam(ICriteria criteria);
    }
}