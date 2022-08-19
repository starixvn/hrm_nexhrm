namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
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

        public Team GetById(GetTeamByIdCriteria criteria)
        {
            Team team = null;
            if (criteria != null && criteria.IsValid())
            {
                team = teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetTeam()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetAll()
        {
            var criteria = new GetAllTeamCriteria
            {
                IncludeSpecialTeam = true
            };
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            IList<Team> teams = baseService.GetData<Team>(criteria);
            return teams;
        }
    }
}