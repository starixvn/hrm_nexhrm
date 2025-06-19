namespace Fanex.NexHR.Services.AutoJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.LogTime;
    using Fanex.NexHR.Services.Interfaces;

    public class BotLogTimeRemindService : IBotLogTimeRemindService
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly IBioWebApiService bioWebApiService;
        private readonly AppSetting appSetting;

        public BotLogTimeRemindService(IBaseService baseService,
            IEmployeeService employeeService,
            INexbotService nexbotService,
            IBioWebApiService bioWebApiService,
            AppSetting appSetting)
        {
            this.baseService = baseService;
            this.nexbotService = nexbotService;
            this.employeeService = employeeService;
            this.bioWebApiService = bioWebApiService;
            this.appSetting = appSetting;
        }

        public async Task<CommandResult> SendMessageLogTimeRemindAsync()
        {
            var yesterday = DateTimeOffset.Now.DateTime.ToClientTimeZone().AddDays(-1);
            if (DateTimeOffset.Now.DateTime.ToClientTimeZone().DayOfWeek == DayOfWeek.Monday)
            {
                yesterday = DateTimeOffset.Now.DateTime.ToClientTimeZone().AddDays(-3);
            }

            var timeEntries = await bioWebApiService.GetTimeEntriesAsync(yesterday);
            var employees = await employeeService.GetEmployeesMissedLogTime().ConfigureAwait(false);
            var ignoreEmployees = (appSetting.IgnoreEmployeeLogTime ?? "").Split(',');
            var ignoreJobTitles = (appSetting.IgnoreJobTitleLogTime ?? "").Split(',');
            var ignoreTeams = (appSetting.IgnoreTeamLogTime ?? "").Split(',');

            employees = employees.Where(e =>
                !Array.Exists(ignoreJobTitles, keyword => e.JobTitle?.Contains(keyword) ?? false) &&
                Array.IndexOf(ignoreEmployees, e.Code ?? string.Empty) < 0 &&
                Array.IndexOf(ignoreTeams, e.TeamCode ?? string.Empty) < 0 &&
                !timeEntries.Any(te => te.Login.Equals(e.DomainUsername?.Split('\\').LastOrDefault(), StringComparison.OrdinalIgnoreCase))
            ).ToList();

            var employeesMissedLogging = employees.Where(e => !e.IsLeader).ToList();
            await SendMessageToEmployees(employeesMissedLogging, yesterday).ConfigureAwait(false);
            await SendMessageToLeaders(employees, yesterday).ConfigureAwait(false);

            var criteria = new UpdateMissedLogTimeCriteria
            {
                EmpIDs = string.Join(",", employeesMissedLogging.Select(e => e.EmpID))
            };
            await baseService.ExecuteNonQueryAsync(criteria);
            return new CommandResult { Code = 0 };
        }

        private async Task SendMessageToLeaders(IEnumerable<EmployeesMissedLogTime> employees, DateTime yesterday)
        {
            var groupedTeams = employees.Where(e => !e.IsLeader).GroupBy(e => e.TeamCode).ToList();

            foreach (var team in groupedTeams)
            {
                var leader = employees.FirstOrDefault(e => e.TeamCode == team.Key && e.IsLeader);
                if (leader == null)
                {
                    continue;
                }
                if (string.IsNullOrWhiteSpace(leader.ConversationId))
                {
                    continue;
                }

                var missedEmployees = team
                    .Where(e => (e.MissedCount + 1) % 3 == 0)
                    .ToList();

                if (missedEmployees.Count == 0)
                {
                    continue;
                }

                var data = new MissedLogTimeLeaderModel
                {
                    ConversationId = leader.ConversationId,
                    LeaderName = leader.NickName,
                    LogMonth = yesterday.ToMonthAndYearFormatedDate(),
                    Members = missedEmployees.Select(e => new MemberModel
                    {
                        NickName = e.NickName,
                        MissedCount = e.MissedCount + 1
                    }).ToList()
                };
                await nexbotService.SendMissedLogTimeRemindLeader(data).ConfigureAwait(false);
            }
        }

        private async Task SendMessageToEmployees(IEnumerable<EmployeesMissedLogTime> employees, DateTime yesterday)
        {
            foreach (var employee in employees)
            {
                if (string.IsNullOrWhiteSpace(employee.ConversationId))
                {
                    continue;
                }

                var data = new MissedLogTimeMemberModel
                {
                    ConversationId = employee.ConversationId,
                    MissedCount = employee.MissedCount + 1,
                    NickName = employee.NickName,
                    LogDate = yesterday.ToDateMonthYearFormatedDate(),
                    LogMonth = yesterday.ToMonthAndYearFormatedDate()
                };
                await nexbotService.SendMissedLogTimeRemindMember(data).ConfigureAwait(false);
            }
        }
    }
}