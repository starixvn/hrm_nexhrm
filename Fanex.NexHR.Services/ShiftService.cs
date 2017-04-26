namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caching;
    using Criteria.Shift;
    using Interfaces;
    using Models;

    public class ShiftService : BaseService, IShiftService
    {
        private const string CacheKeyAllShift = "ShiftService_AllShift";
        private readonly ICache _cacheManager;

        public ShiftService()
        {
            _cacheManager = CacheService.CreateInstance();
        }

        public IList<Shift> GetAll()
        {
            var shifts = _cacheManager.Get<IList<Shift>>(CacheKeyAllShift);
            if (shifts == null || !shifts.Any())
            {
                var criteria = new GetShiftCriteria();
                shifts = GetData<Shift>(criteria);

                if (shifts != null && shifts.Any())
                {
                    _cacheManager.Set(CacheKeyAllShift, shifts, TimeSpan.FromHours(1));
                }
            }

            return shifts;
        }

        public Shift GetByDate(GetShiftByDateCriteria criteria)
        {
            Shift shift = GetDataEntity<Shift>(criteria);
            return shift;
        }
    }
}
