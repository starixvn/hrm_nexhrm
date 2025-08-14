using System;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers.API.Sync;

[ApiController]
[Route("api/sync/[controller]")]
public class JobTitleController(IJobTitleService jobTitleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobTitleByLastModifiedDate(DateTime? lastModifiedDate)
    {
        var criteria = new GetJobTitleByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await jobTitleService.GetJobTitleByLastModifiedDate(criteria);

        return Ok(result);
    }
}