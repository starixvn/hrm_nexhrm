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
            Team team = new Team();
            if (criteria.IsValid())
            {
                team = _teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetAll()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = new List<Team>();
            if (criteria.IsValid())
            {
                teams = _teamRepository.GetAll(criteria);
            }

            return teams;
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

        public int AddNewTeam(AddNewTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int status = -1;
            if (criteria.IsValid())
            {
                status = _teamRepository.AddNewTeam(criteria);
            }

            return status;
        }

        public bool UpdateTeam(UpdateTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = _teamRepository.UpdateTeam(criteria);
            }

            return updated;
        }

        public CommandResult DeleteTeam(DeleteTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}