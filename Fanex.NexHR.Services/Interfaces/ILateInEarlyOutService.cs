namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.LateInEarlyOut;
    using ViewModels.LateInEarlyOut;

    public interface ILateInEarlyOutService
    {
        IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria);

        LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria);

        IList<LateInEarlyOutViewModel> GetAllEmployee(GetAllEmployeeCriteria criteria);
    }
}