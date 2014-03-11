// --------------------------------------------------------------------------
// <copyright file="ShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>
namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// Shift service.
    /// </summary>
    public class ShiftService : IShiftService
    {
        private const string _CacheKeyAllShift = "ShiftService_AllShift";
        private readonly IShiftRepository _shiftRepository;
        private readonly ICache _cacheManager;

        public ShiftService(IShiftRepository shiftRepository = null)
        {
            this._shiftRepository = shiftRepository ?? new ShiftRepository();
            this._cacheManager = CacheService.CreateInstance();
        }

        public ShiftService()
            : this(null)
        { 
        }

        /// <summary>
        /// Get all shifts.
        /// </summary>
        /// <param name="criteria">GetAllShiftCriteria criteria.</param>
        /// <returns>List of Shift.</returns>
        public IList<Shift> GetAll(GetAllShiftCriteria criteria)
        {
            var allShifts = this._cacheManager.Get<IList<Shift>>(_CacheKeyAllShift);
            if (allShifts == null || allShifts.Count <= 0)
            {
                if (criteria.IsValid())
                {
                    allShifts = this._shiftRepository.GetAll(criteria);
                }
                
                if (allShifts != null && allShifts.Count > 0)
                {
                    this._cacheManager.Set<IList<Shift>>(_CacheKeyAllShift, allShifts, TimeSpan.FromHours(1));
                }
            }

            return allShifts;
        }

        /// <summary>
        /// Get select list item.
        /// </summary>
        /// <returns>List of select list item.</returns>
        public IList<SelectListItem> GetSelectList()
        {
            var list = new List<SelectListItem>();
            IList<Shift> shifts = new List<Shift>();
            var getAllShiftCriteria = new GetAllShiftCriteria();
            if (getAllShiftCriteria.IsValid())
            {
                shifts = this.GetAll(getAllShiftCriteria);
            }

            foreach (Shift item in shifts)
            {
                var listItem = new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                };

                list.Add(listItem);
            }

            return list;
        }

        /// <summary>
        /// Get shift by date.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>The shift object.</returns>
        public Shift GetByDate(GetShiftByDateCriteria criteria)
        {
            Shift shift = new Shift();
            if (criteria.IsValid())
            {
                shift = this._shiftRepository.GetByDate(criteria);
            }

            return shift;
        }
    }
}
