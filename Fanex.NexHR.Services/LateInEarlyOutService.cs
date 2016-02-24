namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.LateInEarlyOut;
    using System.Collections.Generic;
    using System.Linq;

    public class LateInEarlyOutService : BaseService, ILateInEarlyOutService
    {
        private readonly ILateInEarlyOutRepository repository;

        public LateInEarlyOutService(ILateInEarlyOutRepository repository = null)
        {
            this.repository = repository ?? new LateInEarlyOutRepository();
        }

        public IList<LateInEarlyOutViewModel> GetAllByEmpId(GetLateInEarlyOutByEmpIdCriteria criteria)
        {
            List<LateInEarlyOutViewModel> list = new List<LateInEarlyOutViewModel>();
            if (criteria != null && criteria.IsValid())
            {
                list = repository.GetAllByEmpId(criteria).ToList();
            }

            return list;
        }

        public LateInEarlyOutViewModel GetById(GetLateInEarlyOutByIdCriteria criteria)
        {
            LateInEarlyOutViewModel model = new LateInEarlyOutViewModel();
            if (criteria != null & criteria.IsValid())
            {
                model = repository.GetById(criteria);
            }
            
            return model;
        }
    }
}
