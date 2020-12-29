namespace Fanex.NexHR.Controllers.API
{
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;

    [Authorize]
    [JwtAuthentication]
    public class SyncDataController : ApiController
    {
        private readonly ISyncDataService syncDataService;
        private readonly DateTime DefaultModifiedDate = new DateTime(2010, 1, 1);
        private const string ModifiedDateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        public SyncDataController() : this(null)
        {
        }

        public SyncDataController(ISyncDataService syncDataService)
        {
            this.syncDataService = syncDataService ?? new SyncDataService();
        }

        public async Task<IHttpActionResult> GetEmployee(bool isNexHrm, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var employees = await syncDataService.GetEmployeeAsync(isNexHrm, modifiedDate);

            return Json(employees);
        }

        public async Task<IHttpActionResult> GetDepartment(bool isNexHrm, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var departments = await syncDataService.GetDepartmentAsync(isNexHrm, modifiedDate);

            return Json(departments);
        }

        public async Task<IHttpActionResult> GetGroup(bool isNexHrm, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var groups = await syncDataService.GetGroupAsync(isNexHrm, modifiedDate);

            return Json(groups);
        }

        public async Task<IHttpActionResult> GetJobTitle(bool isNexHrm)
        {
            var jobTitles = await syncDataService.GetJobTitleAsync(isNexHrm);

            return Json(jobTitles);
        }

        private DateTime GetLastModifiedDate(string dateTimeString)
        {
            DateTime outputDateTime = DateTime.ParseExact(dateTimeString.Replace("T", " "), ModifiedDateTimeFormat, null);

            var modifiedDate = string.IsNullOrWhiteSpace(dateTimeString) ? DefaultModifiedDate : outputDateTime;

            return modifiedDate;
        }
    }
}