namespace Fanex.NexHR.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Data.Repository;
    using Models;

    public interface ITeamRepository
    {
        Team GetById(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);
    }
}
