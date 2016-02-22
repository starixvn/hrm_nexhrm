namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface ILateInEarlyOutRepository
    {
        int Insert(ICriteria criteria);

        int Update(ICriteria criteria);

        int Delete(ICriteria criteria);

        IList<LateInEarlyOut> GetByEmpId(ICriteria criteria);
    }
}
