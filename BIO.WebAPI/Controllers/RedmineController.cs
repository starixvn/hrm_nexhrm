using System;
using System.Threading.Tasks;
using BIO.WebAPI.Criteria;
using BIO.WebAPI.Models;
using BIO.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BIO.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedmineController(IBaseService baseService) : ControllerBase
{
    [HttpGet("time-entries")]
    public async Task<IActionResult> GetTimeEntries([FromQuery] DateTime date)
    {
        var criteria = new GetRedmineTimeEntriesCriteria
        {
            Date = date
        };
        var timeEntries = await baseService.GetDataAsync<RedmineTimeEntry>(criteria);
        return Ok(timeEntries);
    }
}