namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class ShiftService : BaseService, IShiftService
    {
        private const string _CacheKeyAllShift = "ShiftService_AllShift";
        private readonly ICache _cacheManager;

        public ShiftService()
        {
            this._cacheManager = CacheService.CreateInstance();
        }

        public IList<Shift> GetAll()
        {
            var shifts = _cacheManager.Get<IList<Shift>>(_CacheKeyAllShift);
            if (shifts == null || !shifts.Any())
            {
                var criteria = new GetShiftCriteria();
                if (criteria.IsValid())
                {
                    shifts = GetData<Shift>(criteria).ToList();
                }

                if (shifts != null && shifts.Any())
                {
                    _cacheManager.Set<IList<Shift>>(_CacheKeyAllShift, shifts, TimeSpan.FromHours(1));
                }
            }

            return shifts;
        }

        public Shift GetByDate(GetShiftByDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Shift shift = new Shift();
            if (criteria.IsValid())
            {
                shift = GetDataEntity<Shift>(criteria);
            }

            return shift;
        }
    }
}
