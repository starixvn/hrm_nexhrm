namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.SkypeAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models.SkypeAccount;
    using Services.Interfaces;
    using ViewModels.SkypeAccount;

    public class SkypeAccountController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;

        public SkypeAccountController(ISessionService sessionService,
            ITeamService teamService,
            INexbotService nexbotService,
            IBaseService baseService
            ) : base(sessionService)
        {
            this.teamService = teamService;
            this.nexbotService = nexbotService;
            this.baseService = baseService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new SkypeAccountFilter
            {
                Teams = await teamService.GetTeam()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> GetEmployeeSkypeAccount(SkypeAccountFilter viewModel)
        {
            var criteria = new GetEmployeeSkypeAccountCriteria
            {
                EmployeeId = viewModel.EmployeeId,
                GroupId = viewModel.TeamId,
                IsConfirmed = viewModel.IsConfirmed
            };

            var employeeAccounts = await nexbotService.GetEmployeeSkypeAccountAsync(criteria);

            return PartialView("_ListAccount", employeeAccounts);
        }

        [Authorize(Roles = "HR")]
        public async Task<IActionResult> EditAccount(int employeeId, string fullName, string nickName)
        {
            var skypeAccounts = await nexbotService.GetSkypeAccountAsync();

            var viewModel = new SkypeAccountViewModel
            {
                EmployeeId = employeeId,
                FullName = fullName,
                NickName = nickName,
                SkypeUserSuggestion = GetSkypeUserSuggestionSelectList(skypeAccounts, nickName)
            };

            return PartialView("_EditAccount", viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> EditAccount(SkypeAccountViewModel viewModel)
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

                var result = await baseService.ExecuteNonQueryAsync(criteria);
                isSuccessful = result.IsSuccessful;
                message = result.Message;
            }

            var errors = ModelState
                .Where(item => item.Value.Errors.Any())
                .Select(item => new { item.Key, item.Value.Errors.FirstOrDefault()?.ErrorMessage });

            return Json(new { isSuccessful, errors, message });
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
    }
}