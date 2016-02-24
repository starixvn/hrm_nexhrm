namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class JobTitleController : BaseController
    {
        private readonly IJobTitleService jobTitleService;
        private readonly ICommandService commandService;

        public JobTitleController()
            : this(null, null)
        {
        }

        public JobTitleController(IJobTitleService jobTitleService, ICommandService commandService)
        {
            this.jobTitleService = jobTitleService ?? new JobTitleService();
            this.commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Index(string jobTitle = "")
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria() { JobTitle = jobTitle };
            IList<JobTitle> jobTitles = jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_Index", jobTitles);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Insert(string name)
        {
            var insertLookupDataCriteria = new InsertLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };
            CommandResult result = commandService.Insert(insertLookupDataCriteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Update(int id, string name)
        {
            var updateLookupDataCriteria = new UpdateLookupDataCriteria()
            {
                Id = id,
                Name = name
            };
            CommandResult result = commandService.Update(updateLookupDataCriteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria() { Id = id };
            CommandResult result = commandService.Delete(deleteLookupDataCriteria);

            return Json(result);
        }
    }
}