namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.YearlyNexRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.YearlyNexRequest;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class YearlyNexRequestController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IYearlyNexRequestService yearlyNexRequestService;

        public YearlyNexRequestController(
           ITeamService teamService,
           IYearlyNexRequestService yearlyNexRequestService)
        {
            this.teamService = teamService ?? new TeamService();
            this.yearlyNexRequestService = yearlyNexRequestService ?? new YearlyNexRequestService();
        }

        public YearlyNexRequestController()
            : this(null, null) { }

        [Authorize(Roles = "HR")]
        public ActionResult Index()
        {
            IList<Team> teams = teamService.GetTeam();
            var model = new YearlyNexRequestViewModel
            {
                Teams = teams,
                Year = DateTime.Now.Year
        };

            return View(model);
        }

        [HttpPost]
        public ActionResult GetYearlyNexRequest(int? empId, int? teamId, int year)
        {
            int? employeeId = null;
            if (empId.HasValue && empId.Value >= 0)
            {
                employeeId = empId;
            }

            var criteria = new GetYearlyNexRequestCriteria
            {
                EmployeeId = employeeId,
                TeamId = teamId,
                Year = year
            };

            IList<YearlyNexRequest> yearlyNexRequests = yearlyNexRequestService.GetYearlyNexRequest(criteria);

            return PartialView("_ListYearlyNexRequest", yearlyNexRequests);
        }
    }
}