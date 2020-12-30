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

        public async Task<IHttpActionResult> GetEmployee(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var employees = await syncDataService.GetEmployeeAsync(isNexcel, modifiedDate);

            return Json(employees);
        }

        public async Task<IHttpActionResult> GetDepartment(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var departments = await syncDataService.GetDepartmentAsync(isNexcel, modifiedDate);

            return Json(departments);
        }

        public async Task<IHttpActionResult> GetGroup(bool isNexcel, string lastModifiedDate)
        {
            var modifiedDate = GetLastModifiedDate(lastModifiedDate);

            var groups = await syncDataService.GetGroupAsync(isNexcel, modifiedDate);

            return Json(groups);
        }

        public async Task<IHttpActionResult> GetJobTitle(bool isNexcel)
        {
            var jobTitles = await syncDataService.GetJobTitleAsync(isNexcel);

            return Json(jobTitles);
        }

        public async Task<IHttpActionResult> GetFlow(bool isNexcel)
        {
            var flowData = await syncDataService.GetFlowAsync(isNexcel);
            return Json(flowData);
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