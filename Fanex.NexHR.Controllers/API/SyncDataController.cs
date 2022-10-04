namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SyncDataController : ControllerBase
    {
        private readonly ISyncDataService syncDataService;
        private readonly DateTime DefaultModifiedDate = new DateTime(2010, 1, 1);
        private const string ModifiedDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public SyncDataController(ISyncDataService syncDataService)
        {
            this.syncDataService = syncDataService;
        }

        public async Task<IActionResult> GetEmployee(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var employees = await syncDataService.GetEmployeeAsync(isNexcel, modifiedDate);

            return Ok(employees);
        }

        public async Task<IActionResult> GetDepartment(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var departments = await syncDataService.GetDepartmentAsync(isNexcel, modifiedDate);

            return Ok(departments);
        }

        public async Task<IActionResult> GetGroup(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var groups = await syncDataService.GetGroupAsync(isNexcel, modifiedDate);

            return Ok(groups);
        }

        public async Task<IActionResult> GetJobTitle(bool isNexcel)
        {
            var jobTitles = await syncDataService.GetJobTitleAsync(isNexcel);

            return Ok(jobTitles);
        }

        public async Task<IActionResult> GetFlow(bool isNexcel)
        {
            var flowData = await syncDataService.GetFlowAsync(isNexcel);
            return Ok(flowData);
        }

        private DateTime GetLastModifiedDate(string dateTimeString)
        {
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                return DefaultModifiedDate;
            }

            DateTime outputDateTime = DateTime.ParseExact(dateTimeString.Replace("T", " "), ModifiedDateTimeFormat, null);

            return outputDateTime;
        }
    }
}