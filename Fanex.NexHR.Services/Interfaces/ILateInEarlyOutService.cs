namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System.Collections.Generic;

    public interface ILateInEarlyOutService
    {
        CommandResult Insert(InsertLateInEarlyOutCriteria criteria);

        CommandResult Update(UpdateLateInEarlyOutCriteria criteria);

        CommandResult Delete(DeleteLateInEarlyOutCriteria criteria);

        IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);

        LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria);
    }
}