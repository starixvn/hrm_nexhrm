// --------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Smith.Tran" date="2012.7.27">Created</change>
//   <change who="Anders.Le" date="2012.8.27">Cracking code and add fanex.logging</change>
//   <change who="Anders.Le" date="2012.8.27">Add function: don't send mail to user in IgnoreList</change>
// </history>

namespace Fanex.NexHR.CheckInOutReminder
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using RazorEngine;

    /// <summary>
    /// Main program.
    /// </summary>
    public class Program
    {
        private static readonly EmailService _EmailService = new EmailService();
        private static readonly EmployeeRepository _EmployeeRepository = new EmployeeRepository();
        private static readonly string _SenderEmail = ConfigurationManager.AppSettings["Email.Sender"].ToString();
        private static readonly string _SenderName = ConfigurationManager.AppSettings["Email.SenderName"].ToString();
        ////private static readonly string TemplateEmail = ConfigurationManager.AppSettings["EmailTemplate"].ToString();

        // Do not send email forget check in out for these employees
        private static readonly List<string> _IgnoreList = new List<string> { "sean", "joe" };

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                LogService.Info("Start " + DateTime.Now.ToString());
                DbSettingProviderManager.Current.Start();

                // Get employee who forgot check in or check out
                var employeeList = _EmployeeRepository.GetMissingCheckInOutEmployees();
                employeeList = employeeList.Where(s => !_IgnoreList.Contains(s.NickName.ToLower())).ToList();

                if (employeeList.Count > 0)
                {
                    LogService.Info("There is/are " + employeeList.Count + " employee(s) forgot check in/out");

                    var bodyTemplate = ReadTextFile("emails/forgetcheckinginout.cshtml");

                    if (!string.IsNullOrWhiteSpace(bodyTemplate))
                    {
                        foreach (var item in employeeList)
                        {
                            CheckInOutInfo data;
                            string emailContent;
                            CreateEmailBody(bodyTemplate, item, out data, out emailContent);

                            string emailSubject = string.Join(null, _SenderName, " - Forgot ", data.Type);

                            _EmailService.SendEmail(
                                _SenderEmail,
                                _SenderName,
                                receiverEmail: item.Email,
                                mailCc: null,
                                subject: emailSubject,
                                body: emailContent);
                        }
                    }
                    else
                    {
                        LogService.Error("Cannot load email template service emails/forgetcheckinginout.cshtml or it's empty");
                    }
                }
                else 
                {
                    LogService.Info("No one missed check in/out");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Read text file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The text file content.</returns>
        public static string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            LogService.Info("Email teamplate path: " + filePath);

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
                LogService.Error(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Create email body.
        /// </summary>
        /// <param name="bodyTemplate">The body template.</param>
        /// <param name="item">The model.</param>
        /// <param name="data">The check in/out data.</param>
        /// <param name="emailContent">The email content.</param>
        private static void CreateEmailBody(
            string bodyTemplate, 
            MissingCheckInOutModel item, 
            out CheckInOutInfo data, 
            out string emailContent)
        {
            data = new CheckInOutInfo
            {
                Employee = item.NickName,
                Date = item.AttendanceDate.ToShortDateString(),
                Type = item.Type == 1 ? "Check In" : "Check Out",
                Shift = item.ShiftCode,
                Remark = item.Type == 1 ? "Forgot Check In" : "Forgot Check Out"
            };

            emailContent = Razor.Parse(bodyTemplate, data);
        }
    }
}
