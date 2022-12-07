namespace Fanex.NexHR.Controllers.API
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.SkypeAccount;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SkypeMappingController : ControllerBase
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;

        public SkypeMappingController(INexbotService nexbotService, IBaseService baseService)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> AutoMap()
        {
            var notMapAccounts = await baseService.GetDataAsync<Employee>(new GetNotMappedSkypeCriteria());
            var accounts = await nexbotService.GetSkypeAccountAsync();
            foreach (var emp in notMapAccounts)
            {
                var skypeMapped = accounts.FirstOrDefault(a => a.UserName.Equals(emp.Skype));

                if (skypeMapped is not null && !string.IsNullOrWhiteSpace(skypeMapped.ConversationId) && emp.Id > 0)
                {
                    await UpdateSkypeAccount(emp.Id, skypeMapped.ConversationId);
                }
            }

            return Ok();
        }

        private async Task UpdateSkypeAccount(int empId, string conversationId)
        {
            var criteria = new UpdateSkypeAccountNickNameCriteria
            {
                EmployeeId = empId,
                ConversationId = conversationId
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}