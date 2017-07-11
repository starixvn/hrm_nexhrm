namespace AutoJobs
{
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class MailReminderService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly EmailService EmailService = new EmailService();
        private static readonly EmployeeService EmployeeService = new EmployeeService();

        private static readonly string SenderEmail = ConfigurationManager.AppSettings["Email.Sender"];
        private static readonly string SenderName = ConfigurationManager.AppSettings["Email.SenderName"];
        private static readonly string EmailSd = ConfigurationManager.AppSettings["EmailSD"];
        private static readonly string EmailGo = ConfigurationManager.AppSettings["EmailGo"];
        private static readonly string EmailIt = ConfigurationManager.AppSettings["EmailIT"];

        private static readonly string AllowSendForgotCheckInOut = ConfigurationManager.AppSettings["AllowSendForgotCheckInOut"];
        private static readonly string AllowSendMailReminderBirthday = ConfigurationManager.AppSettings["AllowSendMailReminderBirthday"];
        private static readonly string TimeSendMailForgotCheckInOut = ConfigurationManager.AppSettings["TimeSendMailForgotCheckInOut"];
        private static readonly string TimeSendMailReminderBirthday = ConfigurationManager.AppSettings["TimeSendMailReminderBirthday"];

        private static readonly List<string> IgnoreList = new List<string> { "sean", "joe" };
        
        private bool _checkSendMailCheckInOut;
        private bool _checkSendMailReminderBirthday;

        public void ReminderForgotCheckInOut(DateTime today)
        {
            if (bool.Parse(AllowSendForgotCheckInOut))
            {
                if (today.Hour >= int.Parse(TimeSendMailForgotCheckInOut) && today.Hour < (int.Parse(TimeSendMailForgotCheckInOut) + 1))
                {
                    if (!_checkSendMailCheckInOut)
                    {
                        Log.Info("CheckInOut Reminder started at " + DateTime.Now);
                        SendMailReminderForgotCheckInOut();
                        Log.Info("CheckInOut Reminder stopped at " + DateTime.Now);
                        _checkSendMailCheckInOut = true;
                    }
                }
                else
                {
                    _checkSendMailCheckInOut = false;
                }
            }
        }

        public void ReminderBirthdayAndFirstWorkingday(DateTime today)
        {
            if (!bool.Parse(AllowSendMailReminderBirthday) || today.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

            if (today.Hour >= int.Parse(TimeSendMailReminderBirthday) &&
                today.Hour < (int.Parse(TimeSendMailReminderBirthday) + 1))
            {
                if (!_checkSendMailReminderBirthday)
                {
                    Log.Info("Birthday and First working day Reminder started at " + DateTime.Now);

                    SendMailBirthdayAndFirstWorkingDay(today.AddDays(1));

                    if (today.DayOfWeek == DayOfWeek.Friday)
                    {
                        SendMailBirthdayAndFirstWorkingDay(today.AddDays(2));
                    }

                    Log.Info("Birthday and First working day Reminder stopped at " + DateTime.Now);
                    _checkSendMailReminderBirthday = true;
                }
            }
            else
            {
                _checkSendMailReminderBirthday = false;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        private void SendMailReminderForgotCheckInOut()
        {
            try
            {
                // Get employee who forgot check in or check out
                var getMissingCheckInOutCriteria = new GetMissingCheckInOutEmployeesCriteria();
                var employeeList = EmployeeService.GetMissingCheckInOutByEmpId(getMissingCheckInOutCriteria);
                employeeList = employeeList.Where(s => !IgnoreList.Contains(s.NickName.ToLower(CultureInfo.InvariantCulture))).ToList();

                if (employeeList.Count > 0)
                {
                    Log.Info("There is/are " + employeeList.Count + " employee(s) forgot check in/out");

                    var bodyTemplate = ReadTextFile("emails/forgetcheckinginout.cshtml");

                    if (!string.IsNullOrWhiteSpace(bodyTemplate))
                    {
                        foreach (var item in employeeList)
                        {
                            Log.Info(item.Email);
                            string emailContent = CreateEmailBody(bodyTemplate, item);
                            string emailSubject = string.Join(null, SenderName, " - Forgot ", item.MissingType == 1 ? "Check In" : "Check Out");

                            EmailService.SendEmail(
                                SenderEmail,
                                receiverEmail: item.Email,
                                mailCC: null,
                                subject: emailSubject,
                                body: emailContent);
                        }
                    }
                    else
                    {
                        Log.Error("Cannot load email template service emails/forgetcheckinginout.cshtml or it's empty");
                    }
                }
                else
                {
                    Log.Info("No one missed check in/out");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Function to send mail to reminder missing check in out.
        /// </summary>
        /// <param name="forDate">The date which employee have birthday and first working day.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        private void SendMailBirthdayAndFirstWorkingDay(DateTime forDate)
        {
            try
            {
                List<Employee> employeesBirthday = null;
                List<Employee> employeesFirstWorkingday = null;
                var criteria = new GetBirthdayAndFirstWorkingdayCriteria
                {
                    OnDate = forDate
                };

                var lstEmployee = EmployeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
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
            }
        }

        /// <summary>
        /// Function to read file text.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>Content of file.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        private string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            try
            {
                using (var sr = new StreamReader(filePath))
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

        /// <summary>
        /// Create email body.
        /// </summary>
        /// <param name="bodyTemplate">Body template.</param>
        /// <param name="item">MissingCheckInOut Model.</param>
        /// <returns>Content of email body.</returns>
        private static string CreateEmailBody(string bodyTemplate, MissingCheckInOutModel item)
        {
            var data = new CheckInOutInfo
            {
                Employee = item.NickName,
                Date = item.AttendanceDate.ToShortDateString(),
                InOutType = item.MissingType == 1 ? "Check In" : "Check Out",
                Shift = item.ShiftCode,
                Remark = item.MissingType == 1 ? "Forgot Check In" : "Forgot Check Out"
            };

            var emailContent = string.Format(bodyTemplate, data.Employee, data.InOutType, data.InOutType, data.Date, data.Shift, data.Remark);

            return emailContent;
        }

        /// <summary>
        /// Function to send mail reminder birthday.
        /// </summary>
        /// <param name="employeesBirthday">List employees have birthday.</param>
        private void SendmailReminderBirthday(IList<Employee> employeesBirthday)
        {
            var employees = string.Join(
                "<br />",
                employeesBirthday.Select(t => ("- <strong>" + t.NickName + "</strong> (" + t.FullName + "): " + t.DateOfBirth.ToString("dd MMM, yyyy"))));

            string emailSubject = "Reminder: Happy birthday to " + string.Join(", ", employeesBirthday.Select(t => t.NickName));
            var bodyTemplate = ReadTextFile("emails/ReminderBirthday.cshtml");
            var emailContent = string.Format(bodyTemplate, employees);

            EmailService.SendEmail(
                        SenderEmail,
                        receiverEmail: EmailSd + ";" + EmailGo + ";" + EmailIt,
                        mailCC: null,
                        subject: emailSubject,
                        body: emailContent);
        }

        /// <summary>
        /// Function to send mail reminder first working day anniversary.
        /// </summary>
        /// <param name="employeesFirstWorkingday">Employees which have first working day on the date.</param>
        private void SendmailReminderFirstWorkingday(IList<Employee> employeesFirstWorkingday)
        {
            var employees = string.Join(
                "<br />",
                employeesFirstWorkingday.Select(t => ("- <strong>" + t.NickName + "</strong> (" + t.FullName + "): "
                    + (t.JoinedDate?.ToString("dd MMM, yyyy") ?? string.Empty))));

            string emailSubject = "Reminder: First working day anniversary of "
                + string.Join(" - ", employeesFirstWorkingday.Select(t => t.NickName));
            var bodyTemplate = ReadTextFile("emails/ReminderFirstWorkingday.cshtml");
            var emailContent = string.Format(bodyTemplate, employees);

            EmailService.SendEmail(
                        SenderEmail,
                        receiverEmail: EmailSd + ";" + EmailGo + ";" + EmailIt,
                        mailCC: null,
                        subject: emailSubject,
                        body: emailContent);
        }
    }
}
