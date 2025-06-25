using System;
using System.Threading.Tasks;
using BIO.WebAPI.Criteria;
using BIO.WebAPI.Models;
using BIO.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BIO.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeRecordController(IBaseService baseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(DateTime fromDate, DateTime toDate)
    {
        var criteria = new GetTimeRecordCriteria
        {
            FromDate = fromDate,
            ToDate = toDate
        };
        var records = await baseService.GetDataAsync<TimeRecord>(criteria);

        return Ok(records);
    }
}