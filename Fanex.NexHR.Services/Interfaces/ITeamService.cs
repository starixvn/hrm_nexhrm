namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;

    public interface ITeamService
    {
        Team GetById(GetTeamByIdCriteria criteria);

        IList<Team> GetAll(GetTeamCriteria criteria);

        IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);

        int AddNewTeam(AddNewTeamCriteria criteria);

        bool UpdateTeam(UpdateTeamCriteria criteria);

        CommandResult DeleteTeam(DeleteTeamCriteria criteria);
    }
}