namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Criteria.LookupData;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services;
    using Services.Interfaces;

    public class JobTitleController : BaseController
    {
        private readonly IJobTitleService jobTitleService;
        private readonly ICommandService commandService;

        public JobTitleController(ISessionService sessionService,
            IJobTitleService jobTitleService, 
            ICommandService commandService) : base(sessionService)
        {
            this.jobTitleService = jobTitleService;
            this.commandService = commandService;
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View();
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<ActionResult> Index(string jobTitle)
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            IEnumerable<JobTitle> jobTitles = await jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_Index", jobTitles);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Insert(string name)
        {
            var insertLookupDataCriteria = new InsertLookupDataCriteria
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };
            CommandResult result = commandService.ExecuteNonQuery(insertLookupDataCriteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Update(int id, string name)
        {
            var updateLookupDataCriteria = new UpdateLookupDataCriteria
            {
                Id = id,
                Name = name
            };
            CommandResult result = commandService.ExecuteNonQuery(updateLookupDataCriteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria { Id = id };
            CommandResult result = commandService.ExecuteNonQuery(deleteLookupDataCriteria);

            return Json(result);
        }
    }
}