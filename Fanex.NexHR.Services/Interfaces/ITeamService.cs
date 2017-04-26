namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.Team;
    using Models;

    public interface ITeamService
    {
        Team GetById(GetTeamByIdCriteria criteria);

        IList<Team> GetTeam();

        IList<Team> GetAll();

        IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);
    }
}