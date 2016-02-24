namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository teamRepository;

        public TeamService(ITeamRepository teamRepository = null)
        {
            this.teamRepository = teamRepository ?? new TeamRepository();
        }

        public Team GetById(GetTeamByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Team team = null;
            if (criteria.IsValid())
            {
                team = teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetAll()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> teams = null;
            if (criteria.IsValid())
            {
                teams = GetData<Team>(criteria).ToList();
            }

            return teams ?? new List<Team>();
        }
    }
}