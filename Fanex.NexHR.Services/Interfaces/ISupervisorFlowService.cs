namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;

    public interface ISupervisorFlowService
    {
        Task<IEnumerable<TeamSupervisor>> GetTeamSupervisors(GetTeamSupervisorCriteria criteria);

        Task<IEnumerable<PersonalSupervisor>> GetPersonalSupervisors(GetPersonalSupervisorCriteria criteria);

        Task<PersonalSupervisor> GetPersonalSupervisorsById(GetPersonalSupervisorByIdCriteria criteria);

        Task<IEnumerable<DetailSupervisor>> GetListDetailSupervisors(GetDetailSupervisorCriteria criteria);

        Task<TeamSupervisor> GetById(GetTeamSupervisorByIdCriteria criteria);
    }
}
