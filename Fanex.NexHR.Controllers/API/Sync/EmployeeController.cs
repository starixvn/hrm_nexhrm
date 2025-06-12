using System;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers.API.Sync;

[ApiController]
[Route("api/sync/[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployeesByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeesByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await employeeService.GetEmployeesByLastModifiedDate(criteria);

        return new JsonResult(result);
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> GetEmployeeAttendance(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeAttendancesCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await employeeService.GetEmployeeAttendances(criteria);
        return Ok(result);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetEmployeeCalendarsByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeCalendarsByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await employeeService.GetEmployeeCalendarsByLastModifiedDate(criteria);

        return Ok(result);
    }

    [HttpGet("overtime")]
    public async Task<IActionResult> GetEmployeeOvertimesByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeOvertimeByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await employeeService.GetEmployeeOvertimesByLastModifiedDate(criteria);
        return Ok(result);
    }

    [HttpGet("toeic")]
    public async Task<IActionResult> GetEmployeeTOEICsByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeTOEICByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await employeeService.GetEmployeeTOEICsByLastModifiedDate(criteria);
        return Ok(result);
    }
}