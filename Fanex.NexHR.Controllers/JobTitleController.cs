namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using Criteria.LookupData;
    using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Index(string jobTitle)
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            IList<JobTitle> jobTitles = jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_Index", jobTitles);
        }

        [Authorize(Roles = "HR")]
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

        [Authorize(Roles = "HR")]
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria { Id = id };
            CommandResult result = commandService.ExecuteNonQuery(deleteLookupDataCriteria);

            return Json(result);
        }
    }
}