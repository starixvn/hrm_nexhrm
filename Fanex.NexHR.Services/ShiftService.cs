namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caching;
    using Criteria.Shift;
    using Interfaces;
    using Models;

    public class ShiftService : IShiftService
    {
        private const string CacheKeyAllShift = "ShiftService_AllShift";
        private readonly IBaseService baseService;

        public ShiftService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Shift> GetAll()
        {
            var shifts = ConfiguredCacheService.Get<IList<Shift>>(CacheKeyAllShift);
            if (shifts == null || !shifts.Any())
            {
                var criteria = new GetShiftCriteria();
                shifts = baseService.GetData<Shift>(criteria);

                if (shifts != null && shifts.Any())
                {
                    ConfiguredCacheService.Set(CacheKeyAllShift, shifts,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                }
            }

            return shifts;
        }

        public Shift GetByDate(GetShiftByDateCriteria criteria)
        {
            Shift shift = baseService.GetDataEntity<Shift>(criteria);
            return shift;
        }
    }
}