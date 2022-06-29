namespace Fanex.NexHR.Services
{
    using Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BaseService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBaseRepository baseRepository;

        public BaseService()
            : this(null, null)
        {
        }

        public BaseService(
            IDynamicRepository dynamicRepository,
            IBaseRepository baseRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.baseRepository = baseRepository ?? new BaseRepository();
        }

        public IList<T> GetData<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Fetch<T>(criteria).ToList();
            }

            return new List<T>();
        }

        public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria);
            }

            return new List<T>();
        }

        public T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }

        public async Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond>(criteria);
            }

            return new ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(new List<TFirst>(), new List<TSecond>());
        }

        public async Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond, TThird>(criteria);
            }

            return new ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(new List<TFirst>(), new List<TSecond>(), new List<TThird>());
        }

        public async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria);
            }

            return default(T);
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command)
        {
            var commandResult = new CommandResult();

            if (command != null && command.IsValid())
            {
                commandResult = await baseRepository.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = await baseRepository.ExecuteNonQueryAsync(criteria);
            }

            return commandResult;
        }
    }
}