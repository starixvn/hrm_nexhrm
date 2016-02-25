namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : BaseService, ILateInEarlyOutService
    {
        public IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> lateInEarlyOuts = GetData<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOuts;
        }

        public LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = GetDataEntity<LateInEarlyOutViewModel>(criteria);
            return lateInEarlyOut;
        }
    }
}
