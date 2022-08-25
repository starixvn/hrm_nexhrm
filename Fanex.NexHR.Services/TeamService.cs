namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Team;
    using Interfaces;
    using Models;
    using Repositories.Interfaces;

    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IBaseService baseService;

        public TeamService(ITeamRepository teamRepository, IBaseService baseService)
        {
            this.teamRepository = teamRepository;
            this.baseService = baseService;
        }

        public async Task<Team> GetById(GetTeamByIdCriteria criteria)
        {
            Team team = null;
            if (criteria != null && criteria.IsValid())
            {
                team = await teamRepository.GetById(criteria);
            }

            return team;
        }

        public async Task<IList<Team>> GetTeam()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = await teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public async Task<IList<Team>> GetAll()
        {
            var criteria = new GetAllTeamCriteria
            {
                IncludeSpecialTeam = true
            };
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = await teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public async Task<IEnumerable<Team>> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            IEnumerable<Team> teams = await baseService.GetDataAsync<Team>(criteria);
            return teams;
        }
    }
}