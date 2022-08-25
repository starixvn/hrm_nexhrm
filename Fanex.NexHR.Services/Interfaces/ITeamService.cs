namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.Team;
    using Models;

    public interface ITeamService
    {
        Task<Team> GetById(GetTeamByIdCriteria criteria);

        Task<IList<Team>> GetTeam();

        Task<IList<Team>> GetAll();

        Task<IEnumerable<Team>> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);
    }
}