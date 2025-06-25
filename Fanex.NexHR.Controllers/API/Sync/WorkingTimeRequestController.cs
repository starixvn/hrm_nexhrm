using System;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers.API.Sync
{
    [ApiController]
    [Route("api/sync/[controller]")]
    public class WorkingTimeRequestController(IWorkingTimeRequestService workingTimeRequestService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkingTimeRequestsByLastModifiedDate(DateTime lastModifiedDate)
        {
            var criteria = new GetWorkingTimeRequestsByLastModifiedDateCriteria
            {
                LastModifiedDate = lastModifiedDate
            };
            var result = await workingTimeRequestService.GetWorkingTimeRequestsByLastModifiedDate(criteria);
            return Ok(result);
        }
    }
}
