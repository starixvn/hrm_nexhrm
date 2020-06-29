namespace Fanex.NexHR.Services.Email
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using log4net;

    public class MailReminderService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly EmailService EmailService = new EmailService();
        private static readonly EmployeeService EmployeeService = new EmployeeService();
        private static readonly string SenderEmail = ConfigurationManager.AppSettings["Email"];
        private static readonly string EmailSd = ConfigurationManager.AppSettings["EmailSD"];
        private static readonly string EmailGo = ConfigurationManager.AppSettings["EmailGo"];
        private static readonly string EmailNetwork = ConfigurationManager.AppSettings["EmailNetwork"];
        private static readonly string AllowSendMailReminderBirthday = ConfigurationManager.AppSettings["AllowSendMailReminderBirthday"];
        private static readonly string TimeSendMailReminderBirthday = ConfigurationManager.AppSettings["TimeSendMailReminderBirthday"];
        private bool _checkSendMailReminderBirthday;
        private static bool _isReminderBirthdayAndFirstWorkingdaySuccess = true;

        public async Task<CommandResult> ReminderBirthdayAndFirstWorkingday()
        {
            var today = DateTime.Now;
            const int days = 2;

            if (!bool.Parse(AllowSendMailReminderBirthday) || today.DayOfWeek == DayOfWeek.Saturday)
            {
                return new CommandResult { Code = 0 };
            }

            if (today.Hour >= int.Parse(TimeSendMailReminderBirthday) &&
                today.Hour < (int.Parse(TimeSendMailReminderBirthday) + 1))
            {
                if (!_checkSendMailReminderBirthday)
                {
                    Log.Info("Birthday and First working day Reminder started at " + DateTime.Now);

                    await SendMailBirthdayAndFirstWorkingDay(today.AddDays(1)).ConfigureAwait(false);

                    if (today.DayOfWeek == DayOfWeek.Friday)
                    {
                        await SendMailBirthdayAndFirstWorkingDay(today.AddDays(days)).ConfigureAwait(false);
                    }

                    Log.Info("Birthday and First working day Reminder stopped at " + DateTime.Now);
                    _checkSendMailReminderBirthday = true;
                }
            }
            else
            {
                _checkSendMailReminderBirthday = false;
            }

            return new CommandResult { Code = _isReminderBirthdayAndFirstWorkingdaySuccess ? 0 : 1 };
        }

        private static async Task SendMailBirthdayAndFirstWorkingDay(DateTime forDate)
        {
            try
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

                if (employeesBirthday != null && employeesBirthday.Any())
                {
                    SendmailReminderBirthday(employeesBirthday);
                }

                if (employeesFirstWorkingday != null && employeesFirstWorkingday.Any())
                {
                    SendmailReminderFirstWorkingday(employeesFirstWorkingday);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                _isReminderBirthdayAndFirstWorkingdaySuccess = false;
            }
        }

        private static string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var pathCombine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(pathCombine))
            {
                throw new FileNotFoundException(pathCombine);
            }

            try
            {
                using (var sr = new StreamReader(pathCombine))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                return string.Empty;
            }
        }

        private static void SendmailReminderBirthday(IList<Employee> employeesBirthday)
        {
            var employees = string.Join(
                "<br />",
                employeesBirthday.Select(t => ("- <strong>" + t.NickName + "</strong> (" + t.FullName + "): " + t.DateOfBirth.ToString("dd MMM, yyyy"))));

            string emailSubject = "Reminder: Happy birthday to " + string.Join(", ", employeesBirthday.Select(t => t.NickName));
            var bodyTemplate = ReadTextFile("app_data/emails/ReminderBirthday.cshtml");
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
                "<br />",
                employeesFirstWorkingday.Select(t => ("- <strong>" + t.NickName + "</strong> (" + t.FullName + "): "
                    + (t.JoinedDate?.ToString("dd MMM, yyyy") ?? string.Empty))));

            string emailSubject = "Reminder: First working day anniversary of "
                + string.Join(" - ", employeesFirstWorkingday.Select(t => t.NickName));
            var bodyTemplate = ReadTextFile("app_data/emails/ReminderFirstWorkingday.cshtml");
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