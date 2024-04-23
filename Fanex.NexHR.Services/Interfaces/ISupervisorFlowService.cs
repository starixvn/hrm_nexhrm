namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;

    public interface ISupervisorFlowService
    {
        Task<IEnumerable<TeamSupervisor>> GetTeamSupervisors(GetTeamSupervisorCriteria criteria);
    }
}
