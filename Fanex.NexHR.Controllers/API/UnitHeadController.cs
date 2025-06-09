namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.SyncData;
    using Fanex.NexHR.Models.SyncData;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitHeadController : ControllerBase
    {
        private readonly IBaseService baseService;

        public UnitHeadController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IEnumerable<UnitHeadInfo>> GetInfo()
        {
            var criteria = new GetUnitHeadInfoCriteria();
            var infos = await baseService.GetDataAsync<UnitHeadInfo>(criteria);

            return infos;
        }

        [HttpGet]
        public async Task<IEnumerable<UnitHeadDailySchedule>> GetSchedule()
        {
            var criteria = new GetUnitHeadScheduleCriteria
            {
                FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date.AddMonths(1)
            };
            var schedules = await baseService.GetDataAsync<UnitHeadDailySchedule>(criteria);

            return schedules;
        }
    }
}