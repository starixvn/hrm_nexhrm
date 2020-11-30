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

        public SyncDataController() : this(null)
        {
        }

        public SyncDataController(ISyncDataService syncDataService)
        {
            this.syncDataService = syncDataService ?? new SyncDataService();
        }

        public async Task<IHttpActionResult> GetEmployee(DateTime? lastModifiedDate)
        {
            var modifiedDate = lastModifiedDate ?? DefaultModifiedDate;
            var employees = await syncDataService.GetEmployeeAsync(modifiedDate);

            return Json(employees);
        }

        public async Task<IHttpActionResult> GetDepartment(DateTime? lastModifiedDate)
        {
            var modifiedDate = lastModifiedDate ?? DefaultModifiedDate;
            var departments = await syncDataService.GetDepartmentAsync(modifiedDate);

            return Json(departments);
        }

        public async Task<IHttpActionResult> GetGroup(DateTime? lastModifiedDate)
        {
            var modifiedDate = lastModifiedDate ?? DefaultModifiedDate;
            var groups = await syncDataService.GetGroupAsync(modifiedDate);

            return Json(groups);
        }
    }
}