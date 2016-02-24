namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;

    public interface ITeamService
    {
        Team GetById(GetTeamByIdCriteria criteria);

        IList<Team> GetAll();

        IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);
    }
}