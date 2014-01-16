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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class Program
    {
        private static readonly EmailService EmailService = new EmailService();
        private static readonly EmployeeRepository EmployeeRepository = new EmployeeRepository();
        private static readonly string SenderEmail = ConfigurationManager.AppSettings["Email.Sender"].ToString();
        private static readonly string SenderName = ConfigurationManager.AppSettings["Email.SenderName"].ToString();
        private static readonly string TemplateEmail = ConfigurationManager.AppSettings["EmailTemplate"].ToString();

        // Do not send email forget check in out for these employees
        private static readonly List<string> IgnoreList = new List<string> { "sean", "joe" };

        public static void Main(string[] args)
        {
            try
            {
                Fanex.Logging.LogService.Info("Start " + DateTime.Now.ToString());
                Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

                // Get employee who forgot check in or check out
                var employeeList = EmployeeRepository.GetMissingCheckInOutEmployees();
                employeeList = employeeList.Where(s => !IgnoreList.Contains(s.NickName.ToLower())).ToList();

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

                            string emailSubject = string.Join(
                                null,
                               SenderName,
                                " - Forgot ",
                                data.Type);

                            EmailService.SendEmail(
                                SenderEmail,
                                SenderName,
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

        public static string ReadTextFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new System.ArgumentNullException("filePath");
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

        private static void CreateEmailBody(string bodyTemplate, ViewModels.MissingCheckInOutModel item, out CheckInOutInfo data, out string emailContent)
        {
            data = new CheckInOutInfo
            {
                Employee = item.NickName,
                Date = item.AttendanceDate.ToShortDateString(),
                Type = item.Type == 1 ? "Check In" : "Check Out",
                Shift = item.ShiftCode,
                Remark = item.Type == 1 ? "Forgot Check In" : "Forgot Check Out"
            };

            emailContent = RazorEngine.Razor.Parse(bodyTemplate, data);
        }
    }
}
