namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Criteria.Team;
    using Interfaces;
    using Models;
    using Repositories;
    using Repositories.Interfaces;

    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository = null)
        {
            _teamRepository = teamRepository ?? new TeamRepository();
        }

        public Team GetById(GetTeamByIdCriteria criteria)
        {
            Team team = null;
            if (criteria != null && criteria.IsValid())
            {
                team = _teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetTeam()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = _teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetAll()
        {
            var criteria = new GetAllTeamCriteria() 
            {
                IncludeSpecialTeam = true
            };
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = _teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            IList<Team> teams = GetData<Team>(criteria);
            return teams;
        }
    }
}