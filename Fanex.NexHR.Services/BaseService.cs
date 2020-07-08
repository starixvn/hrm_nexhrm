namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;

    public class BaseService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IBaseRepository _baseRepository;

        public BaseService()
            : this(null, null)
        {
        }

        public BaseService(
            IDynamicRepository dynamicRepository,
            IBaseRepository baseRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _baseRepository = baseRepository ?? new BaseRepository();
        }

        public virtual IList<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return _dynamicRepository.Fetch<T>(criteria).ToList();
            }

            return new List<T>();
        }

        public virtual async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await _dynamicRepository.FetchAsync<T>(criteria).ConfigureAwait(false);
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

        public virtual async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await _dynamicRepository.GetAsync<T>(criteria).ConfigureAwait(false);
            }

            return default(T);
        }

        public virtual async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = await _baseRepository.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }
    }
}