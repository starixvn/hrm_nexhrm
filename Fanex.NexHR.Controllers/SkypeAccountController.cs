namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Criteria.SkypeAccount;
    using Models.SkypeAccount;
    using Services;
    using Services.Interfaces;
    using ViewModels.SkypeAccount;
    using System;

    public class SkypeAccountController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly INexbotService nexbotService;
        private readonly ICommandService commandService;

        public SkypeAccountController()
        {
            teamService = new TeamService();
            nexbotService = new NexbotService();
            commandService = new CommandService();
        }

        public ActionResult Index()
        {
            var viewModel = new SkypeAccountFilter
            {
                Teams = teamService.GetTeam()
            };

            return View(viewModel);
        }

        public ActionResult GetEmployeeSkypeAccount(SkypeAccountFilter viewModel)
        {
            var criteria = new GetEmployeeSkypeAccountCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                GroupId = viewModel.TeamId,
                IsConfirmed = viewModel.IsConfirmed
            };

            var employeeAccounts = nexbotService.GetEmployeeSkypeAccount(criteria);

            return PartialView("_ListAccount", employeeAccounts);
        }

        [Authorize(Roles = "HR")]
        public ActionResult EditAccount(int employeeId, string fullName, string nickName)
        {
            var skypeAccounts = nexbotService.GetSkypeAccount();

            var viewModel = new SkypeAccountViewModel
            {
                EmployeeId = employeeId,
                FullName = fullName,
                NickName = nickName,
                SkypeUserSuggestion = GetSkypeUserSuggestionSelectList(skypeAccounts, nickName)
            };

            return PartialView("_EditAccount", viewModel);
        }

        private IEnumerable<SelectListItem> GetSkypeUserSuggestionSelectList(IEnumerable<ConversationUser> suggestionList, string nickName)
        {
            var suggestions = suggestionList.Select(x => new SelectListItem
            {
                Text = x.UserName,
                Value = x.ConversationId,
                Selected = x.NickName != null && x.NickName.Equals(nickName)
            });

            return suggestions.OrderByDescending(s => s.Text.ToLowerInvariant().IndexOf(nickName.ToLowerInvariant(), StringComparison.Ordinal));
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult EditAccount(SkypeAccountViewModel viewModel)
        {
            bool isSuccessful = false;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                var criteria = new UpdateSkypeAccountNickNameCriteria
                {
                    EmployeeId = viewModel.EmployeeId,
                    ConversationId = viewModel.ConversationId
                };

                var result = commandService.Update(criteria);
                isSuccessful = result.IsSuccessful;
                message = result.Message;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            return Json(new { isSuccessful, errors, message });
        }
    }
}