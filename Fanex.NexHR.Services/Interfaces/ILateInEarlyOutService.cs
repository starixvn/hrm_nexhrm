namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System.Collections.Generic;

    public interface ILateInEarlyOutService
    {
        int Insert(InsertLateInEarlyOutCriteria criteria);

        int Update(UpdateLateInEarlyOutCriteria criteria);
        
        int Delete(DeleteLateInEarlyOutCriteria criteria);

        IList<LateInEarlyOutViewModel> GetByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);
    }
}