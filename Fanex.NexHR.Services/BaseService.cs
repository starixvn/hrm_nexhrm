// --------------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.12">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using System.Collections.Generic;

    /// <summary>
    /// The base service.
    /// </summary>
    public class BaseService
    {
        private readonly IBaseRepository _repository;
        private readonly IDynamicRepository _dynamicRepository;

        public BaseService()
            : this(null, null)
        {
        }

        public BaseService(IDynamicRepository dynamicRepository, IBaseRepository repository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return _repository;
            }
        }

        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return _dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria.IsValid())
            {
                return _dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }

        protected virtual CommandResult Execute(ICriteria criteria, bool hasGetOutputParams = false)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult result = null;
            if (criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria, hasGetOutputParams);
            }
            else
            {
                result = GetInvalidInput();
            }

            return result;
        }

        private CommandResult GetInvalidInput()
        {
            return new CommandResult()
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
        }
    }
}
