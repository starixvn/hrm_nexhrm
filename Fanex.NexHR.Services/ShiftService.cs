// --------------------------------------------------------------------------
// <copyright file="ShiftService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>
namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    public class ShiftService : IShiftService
    {
        private const string CacheKeyAllShift = "ShiftService_AllShift";
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

        public IList<Shift> GetAll()
        {
            var allShifts = this._cacheManager.Get<IList<Shift>>(CacheKeyAllShift);
            if (allShifts == null || allShifts.Count <= 0)
            {
                allShifts = this._shiftRepository.GetAll();

                if (allShifts != null && allShifts.Count > 0)
                {
                    this._cacheManager.Set<IList<Shift>>(CacheKeyAllShift, allShifts, TimeSpan.FromHours(1));
                }
            }

            return allShifts;
        }

        public IList<SelectListItem> GetSelectList()
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            var shifts = this.GetAll();

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

        public Shift GetByDate(int employeeId, DateTime date)
        {
            if (employeeId > 0)
            {
                return this._shiftRepository.GetByDate(employeeId, date);
            }

            return null;
        }
    }
}
