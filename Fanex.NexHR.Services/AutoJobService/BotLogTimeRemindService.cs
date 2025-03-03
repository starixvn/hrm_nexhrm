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
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using static NPOI.HSSF.Util.HSSFColor;

    public class BotLogTimeRemindService : IBotLogTimeRemindService
    {
        private readonly INexbotService nexbotService;
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly AppSetting appSetting;
        private readonly string testCon = "a:1PUIjUwYI8CbF6yt5xPwFu-dyfctabmG08RNWhNrn8id1TxZNU94RYyTWISjA0CN1APg17i3lcu1abksDC5Snjc3s2N1qV0hcGpGHQurfOyZ7Q27jgJN2dTm_A9f7_R9_";
        //private readonly string testCon = "a:10BICN4Nk97-YCHuleeAG0IOybfNbN2GCGxrxKm1VMk0esBBF5OHpiEVPfqrD2yY1LdIblV8g7O85VgGm_FyHLeWodqM8OmD6Fak32yZmAWHJF5MwBaX6IW26jCgVYVU6";


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
            var groupedTeams = employees
                .Where(e => !e.IsLeader) 
                .GroupBy(e => e.TeamCode)
                .ToList();
            
            foreach (var team in groupedTeams)
            {
                var leader = employees.FirstOrDefault(e => e.TeamCode == team.Key && e.IsLeader);
                if (leader == null) continue;

                var missedEmployees = team.ToList();

                var missedList = string.Join("\n", missedEmployees
                    .Where(e => (e.MissedCount + 1) % 3 == 0)
                    .Select(e => $"- {e.NickName} ({e.MissedCount + 1} times)"));

                if (string.IsNullOrEmpty(missedList)) continue;
                var message = string.Format(Messages.RemindLogTimeLeader, leader.NickName, missedList);
                await nexbotService.SendMessageAsync(testCon, message).ConfigureAwait(false);
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
                var message = string.Format(Messages.RemindLogTimeMember, employee.MissedCount + 1, employee.NickName, yesterday.ToDateMonthYearFormatedDate());
                await nexbotService.SendMessageAsync(testCon, message).ConfigureAwait(false);
            }
        }
    }
}