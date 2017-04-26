namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria.LookupData;
    using Models;
    using Services;
    using Services.Interfaces;

    public class JobTitleController : BaseController
    {
        private readonly IJobTitleService _jobTitleService;
        private readonly ICommandService _commandService;

        public JobTitleController()
            : this(null, null)
        {
        }

        public JobTitleController(IJobTitleService jobTitleService, ICommandService commandService)
        {
            _jobTitleService = jobTitleService ?? new JobTitleService();
            _commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Index(string jobTitle)
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria { JobTitle = jobTitle };
            IList<JobTitle> jobTitles = _jobTitleService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_Index", jobTitles);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Insert(string name)
        {
            var insertLookupDataCriteria = new InsertLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };
            CommandResult result = _commandService.Insert(insertLookupDataCriteria);

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
            CommandResult result = _commandService.Update(updateLookupDataCriteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria() { Id = id };
            CommandResult result = _commandService.Delete(deleteLookupDataCriteria);

            return Json(result);
        }
    }
}