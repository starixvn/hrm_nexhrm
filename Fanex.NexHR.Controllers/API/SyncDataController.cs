namespace Fanex.NexHR.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.Web.API;

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

        public async Task<IHttpActionResult> GetEmployee(string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);
            var employees = await syncDataService.GetEmployeeAsync(modifiedDate);

            return Json(employees);
        }

        public async Task<IHttpActionResult> GetDepartment(string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var departments = await syncDataService.GetDepartmentAsync(modifiedDate);

            return Json(departments);
        }

        public async Task<IHttpActionResult> GetGroup(string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var groups = await syncDataService.GetGroupAsync(modifiedDate);

            return Json(groups);
        }

        public async Task<IHttpActionResult> GetJobTitle()
        {
            var jobTitles = await syncDataService.GetJobTitleAsync();

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