namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data.Repository;
    using Models;
    using Models.Workload;

    public interface IEmployeeRepository
    {
        Task<Employee> GetById(ICriteria criteria);

        Task<Employee> GetByDomainName(ICriteria criteria);

        Task<Employee> GetByNickName(ICriteria criteria);

        Task<IList<Employee>> Query(ICriteria criteria);

        Task<IList<DashBoardPersonalSpace>> GetDashBoardPersonalSpace(ICriteria criteria);

        Task<IList<DashBoardCompanySpace>> GetDashBoardCompanySpace(ICriteria criteria);

        Task<IList<Employee>> GetEmployeeToeicScore(ICriteria criteria);

        Task<SystemReminderModel> GetEmpployeeHasBirthdayAndFirstWorkingday(ICriteria criteria);

        Task<List<Workload>> GetWorkloadByTeam(ICriteria criteria);
    }
}