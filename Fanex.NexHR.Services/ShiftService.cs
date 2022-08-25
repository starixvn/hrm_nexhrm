namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

        public async Task<IEnumerable<Shift>> GetAll()
        {
            var shifts = ConfiguredCacheService.Get<IEnumerable<Shift>>(CacheKeyAllShift);
            if (shifts == null || !shifts.Any())
            {
                var criteria = new GetShiftCriteria();
                shifts = await baseService.GetDataAsync<Shift>(criteria);

                if (shifts != null && shifts.Any())
                {
                    ConfiguredCacheService.Set(CacheKeyAllShift, shifts,
                        new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));
                }
            }

            return shifts;
        }

        public async Task<Shift> GetByDate(GetShiftByDateCriteria criteria)
        {
            Shift shift = await baseService.GetDataEntityAsync<Shift>(criteria);
            return shift;
        }
    }
}