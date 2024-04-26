namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class SupervisorFlowService : ISupervisorFlowService
    {
        private readonly IBaseService baseService;

        public SupervisorFlowService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<TeamSupervisor>> GetTeamSupervisors(GetTeamSupervisorCriteria criteria)
        {
            var teamSupervisors = await baseService.GetDataAsync<TeamSupervisor>(criteria);
            return teamSupervisors;
        }

        public async Task<TeamSupervisor> GetById(GetTeamSupervisorByIdCriteria criteria)
        {
            var team = await baseService.GetDataEntityAsync<TeamSupervisor>(criteria);
            return team;
        }

        public async Task<IEnumerable<PersonalSupervisor>> GetPersonalSupervisors(GetPersonalSupervisorCriteria criteria)
        {
            var personalSupervisors = await baseService.GetDataAsync<PersonalSupervisor>(criteria);
            return personalSupervisors;
        }
    }
}
