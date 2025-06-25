using System;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers.API.Sync;

[ApiController]
[Route("api/sync/[controller]")]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGroupsByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetGroupsByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await groupService.GetGroupsByLastModifiedDate(criteria);
        return Ok(result);
    }
}