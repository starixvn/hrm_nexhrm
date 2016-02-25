namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;

    public class LateInEarlyOutService : BaseService, ILateInEarlyOutService
    {
        public IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            IList<LateInEarlyOutViewModel> lateInEarlyOuts = new List<LateInEarlyOutViewModel>();
            if (criteria != null && criteria.IsValid())
            {
                lateInEarlyOuts = GetData<LateInEarlyOutViewModel>(criteria).ToList();
            }

            return lateInEarlyOuts;
        }

        public LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel lateInEarlyOut = new LateInEarlyOutViewModel();
            if (criteria != null & criteria.IsValid())
            {
                lateInEarlyOut = GetDataEntity<LateInEarlyOutViewModel>(criteria);
            }

            return lateInEarlyOut;
        }
    }
}
