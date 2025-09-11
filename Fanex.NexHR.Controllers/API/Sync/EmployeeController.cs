using System;
using System.Threading.Tasks;
using Fanex.NexHR.Criteria.SyncData.SyncNexGoal;
using Fanex.NexHR.Models.SyncData.SyncNexGoalModel;
using Fanex.NexHR.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NexHR.Controllers.API.Sync;

[ApiController]
[Route("api/sync/[controller]")]
public class EmployeeController(IBaseService baseService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployeesByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeesByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var employees = await baseService.GetDataAsync<SyncEmployeeModel>(criteria);

        return Ok(employees);
    }

    [HttpGet("attendance")]
    public async Task<IActionResult> GetEmployeeAttendance(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeAttendancesCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var attendances = await baseService.GetDataAsync<SyncEmployeeAttendanceModel>(criteria);

        return Ok(attendances);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetEmployeeCalendarsByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeCalendarsByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var calendars = await baseService.GetDataAsync<EmployeeCalendarModel>(criteria);

        return Ok(calendars);
    }

    [HttpGet("overtime")]
    public async Task<IActionResult> GetEmployeeOvertimesByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeOvertimeByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var overtimes = await baseService.GetDataAsync<EmployeeOverTimeModel>(criteria);

        return Ok(overtimes);
    }

    [HttpGet("toeic")]
    public async Task<IActionResult> GetEmployeeTOEICsByLastModifiedDate(DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeTOEICByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var employees = await baseService.GetDataAsync<EmployeeTOEIC>(criteria);

        return Ok(employees);
    }
}