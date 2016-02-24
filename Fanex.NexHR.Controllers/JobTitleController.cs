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
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.LookupData;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

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

        public ActionResult JobTitleManagement()
        {
            return View();
        }

        public ActionResult JobTitleManagementDetail(string jobTitle = "")
        {
            var lookupDataJobTitleCriteria = new GetJobTitleCriteria() { JobTitle = jobTitle };
            IList<LookupData> jobTitles = _lookupDataService.GetJobTitle(lookupDataJobTitleCriteria);

            return PartialView("_JobTitleManagementDetail", jobTitles);
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