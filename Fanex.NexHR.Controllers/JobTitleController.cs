namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using Services.Interfaces;

    public class JobTitleController : BaseController
    {
        private readonly IJobTitleService jobTitleService;
        private readonly IBaseService baseService;

        public JobTitleController(ISessionService sessionService,
            IJobTitleService jobTitleService,
            IBaseService baseService) : base(sessionService)
        {
            this.jobTitleService = jobTitleService;
            this.baseService = baseService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Index()
        {
            return View();
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Index(string jobTitle)
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            IEnumerable<JobTitle> jobTitles = await jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_Index", jobTitles);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Insert(string name)
        {
            var insertLookupDataCriteria = new InsertLookupDataCriteria
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(insertLookupDataCriteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, string name)
        {
            var updateLookupDataCriteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(updateLookupDataCriteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteLookupDataCriteria);

            return Json(result);
        }
    }
}