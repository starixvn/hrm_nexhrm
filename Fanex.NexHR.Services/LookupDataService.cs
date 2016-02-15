// --------------------------------------------------------------------------
// <copyright file="LookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Lookup data service.
    /// </summary>
    public class LookupDataService : ILookupDataService
    {
        private const string _CacheKeyAllLookUpData = "CacheKey_AllLookUpData";
        private readonly ILookupDataRepository _lookupDataRepository;
        private ICache _cacheManager;

        public LookupDataService(ILookupDataRepository lookupDataRepository = null)
        {
            this._lookupDataRepository = lookupDataRepository ?? new LookupDataRepository();
            this._cacheManager = CacheService.CreateInstance();
        }

        /// <summary>
        /// Method Get All LookupData.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of LookupData Entity.</returns>
        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LookupData> lookupDataList = new List<LookupData>();
            if (criteria.IsValid())
            {
                lookupDataList = this._lookupDataRepository.GetAll(criteria);
            }

            return lookupDataList;
        }

        /// <summary>
        /// Get lookup by code and id.
        /// </summary>
        /// <param name="lookupCode">Lookup code.</param>
        /// <param name="lookupId">Lookup id.</param>
        /// <returns>Lookup data.</returns>
        public LookupData GetById(string lookupCode, int lookupId)
        {
            Check.Argument.IsNotNegativeOrZero(lookupId, "lookupId");
            LookupData lookupData = new LookupData();
            var lookupDataList = this.GetByLookupCode(lookupCode);
            if (lookupDataList != null && lookupDataList.Count > 0)
            {
                lookupData = lookupDataList.Single(data => data.Id == lookupId);
            }

            return lookupData;
        }

        public IList<LookupData> GetJobTitle(GetJobTitleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LookupData> lookUpDataList = new List<LookupData>();
            if (criteria.IsValid())
            {
                lookUpDataList = this._lookupDataRepository.GetJobTitle(criteria);
            }

            return lookUpDataList;
        }

        /// <summary>
        /// Delete LookupData.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool Delete(DeleteLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool check = false;
            if (criteria.IsValid())
            {
                check = this._lookupDataRepository.Delete(criteria);
            }

            if (check)
            {
                this.ReSetJobTitleCache();
            }

            return check;
        }

        /// <summary>
        /// Insert LookupData.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool Insert(InsertLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool check = false;
            if (criteria.IsValid())
            {
                check = this._lookupDataRepository.Insert(criteria);
            }

            if (check)
            {
                this.ReSetJobTitleCache();
            }

            return check;
        }

        /// <summary>
        /// Update LookupData.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool Update(UpdateLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool check = false;
            if (criteria.IsValid())
            {
                check = this._lookupDataRepository.Update(criteria);
            }

            if (check)
            {
                this.ReSetJobTitleCache();
            }

            return check;
        }

        /// <summary>
        /// Reset job title cache.
        /// </summary>
        private void ReSetJobTitleCache() 
        {
            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKeyAllLookUpData);
            var lookUpCode = LookupCodeName.JobTitle.ToLower();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = LookupCodeName.JobTitle };
            var listJobTile = this.GetAll(getAllLookupDataCriteria);
            hashTable[lookUpCode] = listJobTile;

            this._cacheManager.Set<Hashtable>(_CacheKeyAllLookUpData, hashTable, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Get lookup data from hash table which is cached in memory.
        /// </summary>
        /// <param name="lookUpCode">Lookup code.</param>
        /// <returns>List lookup data.</returns>
        private IList<LookupData> GetByLookupCode(string lookUpCode)
        {
            Check.Argument.IsNotEmpty(lookUpCode, "lookUpCode");

            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKeyAllLookUpData);
            lookUpCode = lookUpCode.ToLower();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            if (!hashTable.ContainsKey(lookUpCode))
            {
                var getAllLookupDataCriteria = new GetAllLookupDataCriteria() { LookupCode = lookUpCode };
                var lookUpDataList = this._lookupDataRepository.GetAll(getAllLookupDataCriteria);

                if (lookUpDataList != null && lookUpDataList.Count > 0)
                {
                    hashTable.Add(lookUpCode.ToLower(), lookUpDataList);
                    this._cacheManager.Set<Hashtable>(_CacheKeyAllLookUpData, hashTable, TimeSpan.FromHours(1));
                }
            }

            return (IList<LookupData>)hashTable[lookUpCode];
        }
    }
}