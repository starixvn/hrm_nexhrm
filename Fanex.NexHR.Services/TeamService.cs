namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository = null)
        {
            this._teamRepository = teamRepository ?? new TeamRepository();
        }

        public Team GetById(GetTeamByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Team team = null;
            if (criteria.IsValid())
            {
                team = _teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetAll()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = _teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> listTeam = new List<Team>();
            if (criteria.IsValid())
            {
                listTeam = _teamRepository.GetTeamManagedByEmp(criteria);
            }

            return listTeam;
        }
    }
}