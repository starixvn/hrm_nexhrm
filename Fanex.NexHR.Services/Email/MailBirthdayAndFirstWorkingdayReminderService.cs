namespace Fanex.NexHR.Services.Email
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;

    public class MailBirthdayAndFirstWorkingdayReminderService : IMailBirthdayAndFirstWorkingdayReminderService
    {
        private const string FormatDate = "dd MMM, yyyy";
        private readonly string SenderEmail = ConfigurationManager.AppSettings["Email"];
        private readonly string EmailSd = ConfigurationManager.AppSettings["EmailSD"];
        private readonly string EmailGo = ConfigurationManager.AppSettings["EmailGo"];
        private readonly string EmailNetwork = ConfigurationManager.AppSettings["EmailNetwork"];
        private readonly string AllowSendMailReminderBirthday = ConfigurationManager.AppSettings["AllowSendMailReminderBirthday"];
        private readonly string CompanyMailSign = ConfigurationManager.AppSettings["CompanyMailSign"];
        private readonly IEmailService emailService;
        private readonly IEmployeeService employeeService;
        private readonly IWebHostEnvironment environment;

        private bool isReminderBirthdayAndFirstWorkingdaySuccess = true;

        public MailBirthdayAndFirstWorkingdayReminderService(
            IEmailService emailService,
            IEmployeeService employeeService,
            IWebHostEnvironment environment)
        {
            this.emailService = emailService;
            this.employeeService = employeeService;
            this.environment = environment;
        }

        public async Task<CommandResult> ReminderBirthdayAndFirstWorkingday()
        {
            var today = DateTime.Now;
            const int secondDay = 2;

            if (!bool.Parse(AllowSendMailReminderBirthday) || today.DayOfWeek == DayOfWeek.Saturday)
            {
                return new CommandResult { Code = 0 };
            }

            await SendMailBirthdayAndFirstWorkingDay(today.AddDays(1)).ConfigureAwait(false);
            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                await SendMailBirthdayAndFirstWorkingDay(today.AddDays(secondDay)).ConfigureAwait(false);
            }

            return new CommandResult { Code = isReminderBirthdayAndFirstWorkingdaySuccess ? 0 : 1 };
        }

        private async Task SendMailBirthdayAndFirstWorkingDay(DateTime forDate)
        {
            List<Employee> employeesBirthday = null;
            List<Employee> employeesFirstWorkingday = null;
            var criteria = new GetBirthdayAndFirstWorkingdayCriteria
            {
                OnDate = forDate
            };

            var lstEmployee = await employeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria).ConfigureAwait(false);
            if (lstEmployee != null)
            {
                employeesBirthday = lstEmployee.EmployeeHasBirthday;
                employeesFirstWorkingday = lstEmployee.EmployeeHasFirstWorkingDay;
            }
            else
            {
                isReminderBirthdayAndFirstWorkingdaySuccess = false;
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

        private void SendmailReminderBirthday(IList<Employee> employeesBirthday)
        {
            var employees = string.Join(
                Constants.BrTag,
                employeesBirthday.Select(t => string.Format(Constants.ContentEmployeeWithDay, t.NickName, t.FullName, t.DateOfBirth.ToString(FormatDate))));

            var emailSubject = string.Format(Messages.ReminderHappyBirthdayTo, string.Join(", ", employeesBirthday.Select(t => t.NickName)));
            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForReminderBirthday);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employees,
                CompanyMailSign);

            emailService.SendEmail(
                        SenderEmail,
                        EmailSd + ";" + EmailGo + ";" + EmailNetwork,
                        null,
                        emailSubject,
                        emailContent);
        }

        private void SendmailReminderFirstWorkingday(IList<Employee> employeesFirstWorkingday)
        {
            var employees = string.Join(
                Constants.BrTag,
                employeesFirstWorkingday.Select(t => string.Format(Constants.ContentEmployeeWithDay, t.NickName, t.FullName, t.JoinedDate?.ToString(FormatDate) ?? string.Empty)));

            var emailSubject = string.Format(Messages.ReminderFirstWorkingDayAnniversaryOf, string.Join(" - ", employeesFirstWorkingday.Select(t => t.NickName)));
            var templatePath = Path.Combine(environment.WebRootPath, Constants.EmailTemplatePathForReminderFirstWorkingday);
            var bodyTemplate = Utilities.ReadTextFile(templatePath);
            var emailContent = string.Format(
                bodyTemplate,
                employees,
                CompanyMailSign);

            emailService.SendEmail(
                        SenderEmail,
                        EmailSd + ";" + EmailGo + ";" + EmailNetwork,
                        null,
                        emailSubject,
                        emailContent);
        }
    }
}