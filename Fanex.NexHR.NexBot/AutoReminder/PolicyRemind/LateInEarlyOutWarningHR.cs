namespace Fanex.NexHR.NexBot.AutoReminder.PolicyRemind
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Report;
    using Hangfire;
    using Interfaces;
    using Models.Report;
    using Services;
    using Services.Interfaces;

    public class LateInEarlyOutWarningHR : INexbotEvent
    {
        private readonly IReportService reportService;
        private readonly INexbotService nexbotService;
        private readonly string CronExpression = ConfigurationManager.AppSettings["MissingCheckInOutCronExpression"] ?? "0 8-18 * * 1-5";

        public LateInEarlyOutWarningHR()
        {
            reportService = new ReportService();
            nexbotService = new NexbotService();
        }

        public void Initialize()
        {
            RecurringJob.AddOrUpdate(HangfireCategoryMessageType.LateInEarlyOutWarning.GetDisplayName(),
                            () => SendMessageToConversation(HangfireCategoryMessageType.LateInEarlyOutWarning),
                            CronExpression);
        }

        public async Task SendMessageToConversation(HangfireCategoryMessageType categoryId)
        {
            var conversationIds = await nexbotService.GetRegisteredConversation(categoryId).ConfigureAwait(false);
        }

        private IEnumerable<LateInEarlyOutModel> GetLateInEarlyOutUser()
        {
            IList<LateInEarlyOutModel> lateInEarlyOuts = new List<LateInEarlyOutModel>();
            var criteria = new GetLateInEarlyOutReportCriteria
            {
                Year = DateTime.Now.Year
            };

            return reportService.GetLateInEarlyOut(criteria);
        }

        private IEnumerable<LateInEarlyOutModel> GetDeductionBanlanceUsers()
        {
            var lateInEarlyOuts = GetLateInEarlyOutUser();

            return lateInEarlyOuts.Where(e => e.BalanceCurrentYear >= 240);
        }

        private static string GenerateTakeBalanceDeductionMessage(IEnumerable<LateInEarlyOutModel> lateInEarlyOuts)
        {
            string message = $"Dear All. There is/are {lateInEarlyOuts.Count()} employee(s) need to deduct annual leave balance:{Constants.NewLine}";
            foreach (var item in lateInEarlyOuts)
            {
                message += $"- _{item.Employee.DisplayName}_. Balance: _{item.BalanceCurrentYear}_ {Constants.NewLine}";
            }
            message += $"Please make deduction at https://hrm.nexcel.vn/report/lateinearlyout";
            return message;
        }

        private IEnumerable<LateInEarlyOutModel> GetWarningUsersOnCurrentMonth()
        {
            var lateInEarlyOuts = GetLateInEarlyOutUser();

            return lateInEarlyOuts.Where(x => IsWarningUserInCurrentMonth(x));
        }

        private bool IsWarningUserInCurrentMonth(LateInEarlyOutModel lateInEarlyOut)
        {
            string currentMonth = DateTime.Now.ToString("MMM");
            var violateTimeInCurrentMonth = lateInEarlyOut.GetType().GetProperty($"TimesIn{currentMonth}").GetValue(lateInEarlyOut, null).ToString();
            int time = int.Parse(violateTimeInCurrentMonth);
            return time > 3;
        }

        private IEnumerable<LateInEarlyOutDiscipline> GetViolateLateInEarlyOutDiscipline()
        {
            var today = DateTime.Now;
            var criteria = new GetLateInEarlyOutSummaryCriteria
            {
                FromDate = new DateTime(today.Year, 1, 1),
                ToDate = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)),
                IsUserWarning = true
            };

            var violaters = reportService.GetLateInEarlyOutDiscipline(criteria);

            return violaters.Where(v => v.TotalTimes >= 3);
        }
    }
}