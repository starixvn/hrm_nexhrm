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

    public class LateInEarlyOutService : ILateInEarlyOutService
    {
        private readonly ILateInEarlyOutRepository repository;

        public LateInEarlyOutService(ILateInEarlyOutRepository repository = null)
        {
            this.repository = repository ?? new LateInEarlyOutRepository();
        }

        public int Insert(InsertLateInEarlyOutCriteria criteria)
        {
            int result = 0;
            if (criteria != null)
            {
                result = repository.Insert(criteria);
            }

            return result;
        }

        public int Update(UpdateLateInEarlyOutCriteria criteria)
        {
            int result = 0;
            if (criteria != null)
            {
                result = repository.Update(criteria);
            }

            return result;
        }

        public int Delete(DeleteLateInEarlyOutCriteria criteria)
        {
            int result = 0;
            if (criteria != null)
            {
                result = repository.Delete(criteria);
            }

            return result;
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
