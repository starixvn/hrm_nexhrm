namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LogTime;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using static NPOI.HSSF.Util.HSSFColor;

    public class BotLogTimeRemindService : IBotLogTimeRemindService
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly AppSetting appSetting;
        // Lita local
        private readonly string testCon = "a:1enODRExESjqlZKOU-lSgzu9YfbCQVczwTs84VqXaWtYsTRefm7gbJp_19hCvXVWfclrsrmITnWqPEKSSNP0ycRN5XCNzDzCBMrE_5sMI15dfZd3ZKiDIzCfZ11j6sIUN";
        // Henry local
        //private readonly string testCon = "a:10rtfbre13QNMhvXp3qxeMWc0TQIOBxA51gI1iQkzg4t1ZN0Pyj_FuZYNXZGfCqDA3aZ0P_OMWU38v90sNeODdQvjigM5Nw5G1UgHSuvuMGZnhdcf2qUru-NL46vHJdoG";

        public BotLogTimeRemindService(IBaseService baseService,
            IEmployeeService employeeService,
            INexbotService nexbotService,
            AppSetting appSetting)
        {
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.employeeService = employeeService;
            this.appSetting = appSetting;

        }

        public async Task<CommandResult> SendMessageLogTimeRemindAsync()
        {
            var employees = await employeeService.GetEmployeesMissedLogTime().ConfigureAwait(false);
            var ignoreEmployees = appSetting.IgnoreEmployeeLogTime.Split(',');
            var ignoreTeams = appSetting.IgnoreTeamLogTime.Split(',');
            employees = employees.Where(e => 
                    //e.TeamCode is "ZUR" or "NS" &&
                    Array.IndexOf(ignoreEmployees, e.EmpID.ToString()) < 0 &&
                    Array.IndexOf(ignoreTeams, e.TeamCode) < 0)
                .ToList();
            var employeesMissedLogging = employees.Where(e => !e.IsLeader).ToList();
            await SendMessageToEmployees(employeesMissedLogging).ConfigureAwait(false);
            await SendMessageToLeaders(employees).ConfigureAwait(false);

            var criteria = new UpdateMissedLogTimeCriteria
            {
                EmpIDs = string.Join(",", employeesMissedLogging.Select(e => e.EmpID))
            };
            await baseService.ExecuteNonQueryAsync(criteria);
            return new CommandResult { Code = 0 };
        }

        private async Task SendMessageToLeaders(IEnumerable<EmployeesMissedLogTime> employees)
        {
            var groupedTeams = employees.Where(e => !e.IsLeader).GroupBy(e => e.TeamCode).ToList();

            foreach (var team in groupedTeams)
            {
                var leader = employees.FirstOrDefault(e => e.TeamCode == team.Key && e.IsLeader);
                if (leader == null) continue;

                var missedEmployees = team.Where(e => (e.MissedCount + 1) % 3 == 0).ToList();
                if (missedEmployees.Count == 0) continue;

                var data = new MissedLogTimeLeaderModel
                {
                    ConversationId = testCon,
                    LeaderName = leader.NickName,
                    Members = missedEmployees.Select(e => new MemberModel
                    {
                        NickName = e.NickName,
                        MissedCount = e.MissedCount + 1
                    }).ToList()
                };
                await nexbotService.SendMissedLogTimeRemindLeader(data).ConfigureAwait(false);
            }
        }

        private async Task SendMessageToEmployees(IEnumerable<EmployeesMissedLogTime> employees)
        {
            var yesterday = DateTimeOffset.Now.DateTime.AddDays(-1);
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                yesterday = DateTimeOffset.Now.DateTime.AddDays(-3);
            }
            foreach (var employee in employees)
            {
                var data = new MissedLogTimeMemberModel
                {
                    ConversationId = testCon,
                    MissedCount = employee.MissedCount + 1,
                    NickName = employee.NickName,
                    LogDate = yesterday.ToDateMonthYearFormatedDate()
                };
                await nexbotService.SendMissedLogTimeRemindMember(data).ConfigureAwait(false);
            }
        }
    }
}