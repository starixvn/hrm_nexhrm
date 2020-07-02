namespace Fanex.NexHR.Services.Email
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;

    public class MailBirthdayAndFirstWorkingdayReminderService
    {
        private static readonly EmailService EmailService = new EmailService();
        private static readonly EmployeeService EmployeeService = new EmployeeService();
        private static readonly string SenderEmail = ConfigurationManager.AppSettings["Email"];
        private static readonly string EmailSd = ConfigurationManager.AppSettings["EmailSD"];
        private static readonly string EmailGo = ConfigurationManager.AppSettings["EmailGo"];
        private static readonly string EmailNetwork = ConfigurationManager.AppSettings["EmailNetwork"];
        private static readonly string AllowSendMailReminderBirthday = ConfigurationManager.AppSettings["AllowSendMailReminderBirthday"];
        private static bool _isReminderBirthdayAndFirstWorkingdaySuccess = true;

        public async Task<CommandResult> ReminderBirthdayAndFirstWorkingday()
        {
            var today = DateTime.Now;
            const int days = 2;

            if (!bool.Parse(AllowSendMailReminderBirthday) || today.DayOfWeek == DayOfWeek.Saturday)
            {
                return new CommandResult { Code = 0 };
            }

            await SendMailBirthdayAndFirstWorkingDay(today.AddDays(1)).ConfigureAwait(false);
            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                await SendMailBirthdayAndFirstWorkingDay(today.AddDays(days)).ConfigureAwait(false);
            }

            return new CommandResult { Code = _isReminderBirthdayAndFirstWorkingdaySuccess ? 0 : 1 };
        }

        private static async Task SendMailBirthdayAndFirstWorkingDay(DateTime forDate)
        {
            List<Employee> employeesBirthday = null;
            List<Employee> employeesFirstWorkingday = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = forDate
            };

            var lstEmployee = await EmployeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria).ConfigureAwait(false);
            if (lstEmployee != null)
            {
                employeesBirthday = lstEmployee.EmployeeHasBirthday;
                employeesFirstWorkingday = lstEmployee.EmployeeHasFirstWorkingDay;
            }
            else
            {
                _isReminderBirthdayAndFirstWorkingdaySuccess = false;
            }

            if (employeesBirthday != null && employeesBirthday.Any())
            {
                SendmailReminderBirthday(employeesBirthday);
            }

            if (employeesFirstWorkingday != null && employeesFirstWorkingday.Any())
            {
                SendmailReminderFirstWorkingday(employeesFirstWorkingday);
            }
        }

        private static void SendmailReminderBirthday(IList<Employee> employeesBirthday)
        {
            var employees = string.Join(
                Constants.BrTag,
                employeesBirthday.Select(t => string.Format(Constants.ContentEmployeeWithDay, t.NickName, t.FullName, t.DateOfBirth.ToString("dd MMM, yyyy"))));

            var emailSubject = string.Format(Messages.ReminderHappyBirthdayTo, string.Join(", ", employeesBirthday.Select(t => t.NickName)));
            var templatePath = Path.Combine(HttpRuntime.AppDomainAppPath, Constants.EmailTemplatePathForReminderBirthday);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employees,
                ConfigurationManager.AppSettings["CompanyMailSign"]);

            EmailService.SendEmail(
                        SenderEmail,
                        EmailSd + ";" + EmailGo + ";" + EmailNetwork,
                        null,
                        emailSubject,
                        emailContent);
        }

        private static void SendmailReminderFirstWorkingday(IList<Employee> employeesFirstWorkingday)
        {
            var employees = string.Join(
                Constants.BrTag,
                employeesFirstWorkingday.Select(t => string.Format(Constants.ContentEmployeeWithDay, t.NickName, t.FullName, t.JoinedDate?.ToString("dd MMM, yyyy") ?? string.Empty)));

            string emailSubject = string.Format(Messages.ReminderFirstWorkingDayAnniversaryOf, string.Join(" - ", employeesFirstWorkingday.Select(t => t.NickName)));
            var templatePath = Path.Combine(HttpRuntime.AppDomainAppPath, Constants.EmailTemplatePathForReminderFirstWorkingday);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employees,
                ConfigurationManager.AppSettings["CompanyMailSign"]);

            EmailService.SendEmail(
                        SenderEmail,
                        EmailSd + ";" + EmailGo + ";" + EmailNetwork,
                        null,
                        emailSubject,
                        emailContent);
        }
    }
}