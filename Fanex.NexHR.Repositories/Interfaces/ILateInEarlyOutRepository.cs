namespace Fanex.NexHR.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System.Collections.Generic;

    public interface ILateInEarlyOutRepository
    {
        int Insert(ICriteria criteria);

        int Update(ICriteria criteria);

        int Delete(ICriteria criteria);

        IList<LateInEarlyOutViewModel> GetAllByEmpId(ICriteria criteria);

        LateInEarlyOutViewModel GetById(ICriteria criteria);
    }
}
