namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.JobTitle;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;

    public class JobTitleController(
        ISessionService sessionService,
        IBaseService baseService) : BaseController(sessionService)
    {
        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Index()
        {
            return View();
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Index(string jobTitle)
        {
            var criteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            var jobTitles = await baseService.GetDataAsync<JobTitle>(criteria);

            return PartialView("_Index", jobTitles);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var criteria = new InsertJobTitleCriteria
            {
                Name = name
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, string name)
        {
            var criteria = new UpdateJobTitleCriteria
            {
                JobTitleId = id,
                Name = name
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var criteria = new DeleteJobTitleCriteria { JobTitleId = id };
            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }
    }
}