namespace Fanex.NexHR.Controllers
{
    using System;
    using Fanex.NexHR.Criteria.History;
    using Fanex.NexHR.Services.Interfaces;
    using System.Threading.Tasks;
    using Fanex.NexHR.ViewModels.History;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Criteria.LookupData;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class HistoryController(ISessionService sessionService,
        ITeamService teamService,
        IHistoryService historyService,
        IBaseService baseService,
        IJobTitleService jobTitleService,
        IPromotionService promotionService) : BaseController(sessionService)
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FilterHistoryViewModel
            {
                FromDate = DateTimeOffset.Now.Date,
                ToDate = DateTimeOffset.Now.Date,
                Teams = await teamService.GetTeam()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetHistory(FilterHistoryViewModel model)
        {
            var criteria = new GetHistoryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmpId = model.EmployeeId,
                GroupId = model.TeamId
            };
            var history = await historyService.GetAll(criteria);

            return PartialView("_ListHistory", history);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            var history = new CreateHistoryViewModel
            {
                Teams = await teamService.GetTeam(),
                JobTitles = await jobTitleService.GetJobTitle(new GetJobTitleCriteria { JobTitle = "" }),
                JobGrades = await promotionService.GetJobGrade()
            };

            return PartialView(history);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreateHistoryViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertHistoryCriteria
                {
                    EmpId = model.EmpId,
                    GroupId = model.GroupId,
                    JobGradeId = model.JobGradeId,
                    JobTitleId = model.JobTitleId,
                    AffectedDate = model.AffectedDate,
                    ModifiedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var history = await historyService.GetById(new GetHistoryByIdCriteria { Id = id });
            var model = new EditHistoryViewModel();
            if (history != null)
            {
                model.Id = history.Id;
                model.EmpId = history.EmployeeId;
                model.Employee = history.NickName + " (" + history.FullName + ")";
                model.GroupId = history.GroupId;
                model.JobGradeId = history.JobGradeId;
                model.JobTitleId = history.JobTitleId;
                model.AffectedDate = history.AffectedDate;
                model.Teams = await teamService.GetTeam();
                model.JobTitles = await jobTitleService.GetJobTitle(new GetJobTitleCriteria { JobTitle = "" });
                model.JobGrades = await promotionService.GetJobGrade();
            }

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditHistoryViewModel model)
        {
            var criteria = new UpdateHistoryCriteria
            {
                Id = model.Id,
                EmpId = model.EmpId,
                GroupId = model.GroupId,
                JobGradeId = model.JobGradeId,
                JobTitleId = model.JobTitleId,
                AffectedDate = model.AffectedDate,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var deleteHistoryCriteria = new DeleteHistoryCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteHistoryCriteria);

            return result;
        }
    }
}
