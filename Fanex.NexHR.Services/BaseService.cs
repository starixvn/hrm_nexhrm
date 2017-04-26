namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Data.Repository;

    public class BaseService
    {
        private readonly IDynamicRepository _dynamicRepository;

        public BaseService()
            : this(null)
        {
        }

        public BaseService(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public virtual IList<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return _dynamicRepository.Fetch<T>(criteria).ToList();
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return _dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }
    }
}
