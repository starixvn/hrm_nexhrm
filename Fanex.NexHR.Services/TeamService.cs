namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Team;
    using Interfaces;
    using Models;

    public class TeamService : ITeamService
    {
        private readonly IBaseService baseService;

        public TeamService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<Team> GetById(GetTeamByIdCriteria criteria)
        {
            var team = await baseService.GetDataEntityAsync<Team>(criteria);
            return team;
        }

        public async Task<IList<Team>> GetTeam()
        {
            var criteria = new GetTeamCriteria();
            var teams = await baseService.GetDataAsync<Team>(criteria);
            return teams.ToList();
        }

        public async Task<IList<Team>> GetAll()
        {
            var criteria = new GetAllTeamCriteria
            {
                IncludeSpecialTeam = true
            };
            var teams = await baseService.GetDataAsync<Team>(criteria);
            return teams.ToList();
        }

        public async Task<IEnumerable<Team>> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            IEnumerable<Team> teams = await baseService.GetDataAsync<Team>(criteria);
            return teams;
        }
    }
}