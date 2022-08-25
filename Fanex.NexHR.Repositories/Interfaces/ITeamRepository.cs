namespace Fanex.NexHR.Repositories.Interfaces
{
    using Data.Repository;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITeamRepository
    {
        Task<Team> GetById(ICriteria criteria);

        Task<IList<Team>> GetAll(ICriteria criteria);
    }
}
