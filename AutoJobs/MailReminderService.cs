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
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class MailReminderService
    {
        private static readonly ILog _LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly EmailService _EmailService = new EmailService();
        private static readonly EmployeeService _EmployeeService = new EmployeeService();

        private static readonly string _SenderEmail = ConfigurationManager.AppSettings["Email.Sender"].ToString();
        private static readonly string _SenderName = ConfigurationManager.AppSettings["Email.SenderName"].ToString();
        private static readonly string _EmailSD = ConfigurationManager.AppSettings["EmailSD"].ToString();
        private static readonly string _EmailGO = ConfigurationManager.AppSettings["EmailGo"].ToString();
        private static readonly string _EmailIT = ConfigurationManager.AppSettings["EmailIT"].ToString();

        private static readonly string _AllowSendForgotCheckInOut = ConfigurationManager.AppSettings["AllowSendForgotCheckInOut"].ToString();
        private static readonly string _AllowSendMailReminderBirthday = ConfigurationManager.AppSettings["AllowSendMailReminderBirthday"].ToString();
        private static readonly string _TimeSendMailForgotCheckInOut = ConfigurationManager.AppSettings["TimeSendMailForgotCheckInOut"].ToString();
        private static readonly string _TimeSendMailReminderBirthday = ConfigurationManager.AppSettings["TimeSendMailReminderBirthday"].ToString();

        private static readonly List<string> _IgnoreList = new List<string> { "sean", "joe" };
        
        private bool checkSendMailCheckInOut = false;
        private bool checkSendMailReminderBirthday = false;

        public void ReminderForgotCheckInOut(DateTime today)
        {
            if (bool.Parse(_AllowSendForgotCheckInOut))
            {
                if (today.Hour >= int.Parse(_TimeSendMailForgotCheckInOut) && today.Hour < (int.Parse(_TimeSendMailForgotCheckInOut) + 1))
                {
                    if (!checkSendMailCheckInOut)
                    {
                        _LOG.Info("CheckInOut Reminder started at " + DateTime.Now.ToString());
                        SendMailReminderForgotCheckInOut();
                        _LOG.Info("CheckInOut Reminder stopped at " + DateTime.Now.ToString());
                        checkSendMailCheckInOut = true;
                    }
                }
                else
                {
                    checkSendMailCheckInOut = false;
                }
            }
        }

        public void ReminderBirthdayAndFirstWorkingday(DateTime today)
        {
            if (bool.Parse(_AllowSendMailReminderBirthday))
            {
                //// Satuday don't send, becase it was sent on Friday.
                if (today.DayOfWeek != DayOfWeek.Saturday)
                {
                    if (today.Hour >= int.Parse(_TimeSendMailReminderBirthday) && today.Hour < (int.Parse(_TimeSendMailReminderBirthday) + 1))
                    {
                        if (!checkSendMailReminderBirthday)
                        {
                            _LOG.Info("Birthday and First working day Reminder started at " + DateTime.Now.ToString());

                            SendMailBirthdayAndFirstWorkingDay(today.AddDays(1));

                            if (today.DayOfWeek == DayOfWeek.Friday)
                            {
                                SendMailBirthdayAndFirstWorkingDay(today.AddDays(2));
                            }

                            _LOG.Info("Birthday and First working day Reminder stopped at " + DateTime.Now.ToString());
                            checkSendMailReminderBirthday = true;
                        }
                    }
                    else
                    {
                        checkSendMailReminderBirthday = false;
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        private void SendMailReminderForgotCheckInOut()
        {
            try
            {
                // Get employee who forgot check in or check out
                var getMissingCheckInOutCriteria = new GetMissingCheckInOutEmployeesCriteria();
                var employeeList = _EmployeeService.GetMissingCheckInOutByEmpId(getMissingCheckInOutCriteria);
                employeeList = employeeList.Where(s => !_IgnoreList.Contains(s.NickName.ToLower())).ToList();

                if (employeeList.Count > 0)
                {
                    _LOG.Info("There is/are " + employeeList.Count + " employee(s) forgot check in/out");

                    var bodyTemplate = ReadTextFile("emails/forgetcheckinginout.cshtml");

                    if (!string.IsNullOrWhiteSpace(bodyTemplate))
                    {
                        foreach (var item in employeeList)
                        {
                            _LOG.Info(item.Email);
                            string emailContent = CreateEmailBody(bodyTemplate, item);
                            string emailSubject = string.Join(null, _SenderName, " - Forgot ", item.MissingType == 1 ? "Check In" : "Check Out");

                            _EmailService.SendEmail(
                                _SenderEmail,
                                receiverEmail: item.Email,
                                mailCC: null,
                                subject: emailSubject,
                                body: emailContent);
                        }
                    }
                    else
                    {
                        _LOG.Error("Cannot load email template service emails/forgetcheckinginout.cshtml or it's empty");
                    }
                }
                else
                {
                    _LOG.Info("No one missed check in/out");
                }
            }
            catch (Exception ex)
            {
                _LOG.Error(ex.Message, ex);
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
                SystemReminderModel lstEmployee = null;
                List<Employee> employeesBirthday = null;
                List<Employee> employeesFirstWorkingday = null;
                var criteria = new GetBirthdayAndFirstWorkingdayCriteria
                {
                    OnDate = forDate
                };

                lstEmployee = _EmployeeService.GetEmpployeeHasBirthdayAndFirstWorkingday(criteria);
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
                _LOG.Error(ex.Message, ex);
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
                throw new System.ArgumentNullException("filePath");
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
                _LOG.Error(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Create email body.
        /// </summary>
        /// <param name="bodyTemplate">Body template.</param>
        /// <param name="item">MissingCheckInOut Model.</param>
        /// <returns>Content of email body.</returns>
        private string CreateEmailBody(string bodyTemplate, MissingCheckInOutModel item)
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

            _EmailService.SendEmail(
                        _SenderEmail,
                        receiverEmail: _EmailSD + ";" + _EmailGO + ";" + _EmailIT,
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
                    + (t.JoinedDate.HasValue ? t.JoinedDate.Value.ToString("dd MMM, yyyy") : string.Empty))));

            string emailSubject = "Reminder: First working day anniversary of "
                + string.Join(" - ", employeesFirstWorkingday.Select(t => t.NickName));
            var bodyTemplate = ReadTextFile("emails/ReminderFirstWorkingday.cshtml");
            var emailContent = string.Format(bodyTemplate, employees);

            _EmailService.SendEmail(
                        _SenderEmail,
                        receiverEmail: _EmailSD + ";" + _EmailGO + ";" + _EmailIT,
                        mailCC: null,
                        subject: emailSubject,
                        body: emailContent);
        }
    }
}
