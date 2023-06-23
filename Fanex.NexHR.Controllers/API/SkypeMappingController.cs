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
    using Newtonsoft.Json;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SkypeMappingController : ControllerBase
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly ILogServiceProvider logServiceProvider;

        public SkypeMappingController(INexbotService nexbotService,
            IBaseService baseService,
            ILogServiceProvider logServiceProvider)
        {
            this.nexbotService = nexbotService;
            this.baseService = baseService;
            this.logServiceProvider = logServiceProvider;
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
                    WriteLogFile(emp.Id, emp.NickName, skypeMapped.ConversationId);
                    await UpdateSkypeAccount(emp.Id, skypeMapped.ConversationId);
                }

                var telegramAccount = accounts.FirstOrDefault(a => a.UserName.Equals(emp.Yahoo));
                if (telegramAccount is not null && !string.IsNullOrWhiteSpace(telegramAccount.ConversationId) && emp.Id > 0)
                {
                    WriteLogFile(emp.Id, emp.NickName, telegramAccount.ConversationId);
                    await UpdateSkypeAccount(emp.Id, telegramAccount.ConversationId);
                }
            }

            return Ok();
        }

        private void WriteLogFile(int empId, string nickName, string conversationId)
        {
            logServiceProvider.LogFile(JsonConvert.SerializeObject(new
            {
                EmpId = empId,
                nickName,
                conversationId
            }));
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