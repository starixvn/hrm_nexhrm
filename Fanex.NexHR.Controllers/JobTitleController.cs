// --------------------------------------------------------------------------
// <copyright file="JobTitleController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Edward.Nguyen" date="2012.11.8">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    [Authorize]
    public class JobTitleController : BaseController
    {
        private readonly ILookupDataService _lookupDataService;

        public JobTitleController()
            : this(null)
        {
        }

        public JobTitleController(ILookupDataService lookupDataService)
        {
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        [HttpGet]
        public ActionResult JobTitleManagement()
        {
            return View();
        }

        [HttpPost]
        public JsonResult JobTitleManagement(string jobTitle = "")
        {
            ViewBag.JobTitle = jobTitle;

            var lookupDataJobTitleCriteria = new GetLookupDataJobTitleCriteria() { JobTitle = jobTitle };
            var data = _lookupDataService.GetJobTitle(lookupDataJobTitleCriteria);

            return Json(new { data = data }, JsonRequestBehavior.DenyGet);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool DeleteJobTitle(int id)
        {
            var deleteLookupDataCriteria = new DeleteLookupDataCriteria() { Id = id };
            bool deleted = _lookupDataService.Delete(deleteLookupDataCriteria);

            return deleted;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool InsertJobTitle(string name)
        {
            bool inserted = false;
            var insertLookupDataCriteria = new InsertLookupDataCriteria()
            {
                LookupCode = LookupCodeName.JobTitle,
                Name = name
            };

            inserted = _lookupDataService.Insert(insertLookupDataCriteria);

            return inserted;
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public bool UpdatetJobTitle(int id, string name)
        {
            bool updated = false;
            var updateLookupDataCriteria = new UpdateLookupDataCriteria()
            {
                Id = id,
                Name = name
            };

            updated = _lookupDataService.Update(updateLookupDataCriteria);

            return updated;
        }
    }
}