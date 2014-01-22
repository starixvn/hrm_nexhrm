// --------------------------------------------------------------------------
// <copyright file="RequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Request service.
    /// </summary>
    public class RequestService : IRequestService
    {
        private const string MsgLeave = "<span class='req leave'>{1}</span> {0} days";

        private const string MsgLeaveHalfDay = "<span class='req leave'>{0} ({1}-{2})</span> 0.5 day";

        private const string MsgLeaveOneDay = "<span class='req leave'>{0}</span> 1 day";

        private const string MsgChangeShift = "<span class='req changeshift'>{0} <span class='arr'>&rarr;</span> {1}</span> {2}";

        private const string MsgCheckInOut = "<span class='req checkinout'>{0} {1}</span> {2}";

        private const string MsgLateIn = "<span class='req inout'>+{0}' {1}</span> {2}";

        private const string MsgEarlyOut = "<span class='req inout'>-{0}' {1}</span> {2}";

        private const string MailContentLeave = "{0} to {1} ({2} days)";
        private const string MailContentLeaveOneDay = "{0}";
        private const string MailContentLeaveHalfDay = "{0} (0.5 day from {1} to {2})";

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly IShiftService _shiftService;

        public RequestService()
        {
            this._employeeRepository = new EmployeeRepository();
            this._emailService = new EmailService();
            this._leaveTypeService = new LeaveTypeService();
            this._shiftService = new ShiftService();
        }

        public RequestService(IEmployeeRepository employeeRepository, IEmailService emailService, ILeaveTypeService leaveTypeService)
        {
            this._employeeRepository = employeeRepository;
            this._emailService = emailService;
            this._leaveTypeService = leaveTypeService;
        }

        public static string ToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            switch (request.Type.Id)
            {
                case (int)RequestTypes.Leave:
                    if (request.TotalDays > 1)
                    {
                        msg.AppendFormat(MsgLeave, Convert.ToInt32(request.TotalDays), GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    }
                    else if (request.TotalDays == 1)
                    {
                        msg.AppendFormat(MsgLeaveOneDay, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    }
                    else
                    {
                        // 0.5 day
                        msg.AppendFormat(MsgLeaveHalfDay, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate), request.FromDate.ToString("HH:mm"), request.ToDate.ToString("HH:mm"));
                    }

                    break;

                case (int)RequestTypes.ChangeShift:
                    msg.AppendFormat(MsgChangeShift, request.FromShift.Name, request.ToShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.CheckIn:
                case (int)RequestTypes.CheckOut:
                    msg.AppendFormat(MsgCheckInOut, request.FromDate.ToString("HH:mm"), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestTypes.LateIn:
                    msg.AppendFormat(MsgLateIn, request.TotalMins, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestTypes.EarlyOut:
                    msg.AppendFormat(MsgEarlyOut, request.TotalMins, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                default:
                    break;
            }

            return msg.ToString();
        }

        public bool CheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                // update date & time
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);
                int errorCode;
                string message;

                var requestId = this._employeeRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    model.Type,
                    false,
                    null,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    model.ForDate);

                // check result
                if (errorCode == 0)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        // send email(s)
                        var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOut.Request.cshtml"));
                        foreach (var item in leaders)
                        {
                            var content = new
                            {
                                Leader = item.NickName,
                                Member = model.DisplayName,
                                Type = model.Type == (short)RequestTypes.CheckIn
                                    ? RequestTypes.CheckIn.EnumToString().ToLower()
                                    : RequestTypes.CheckOut.EnumToString().ToLower(),
                                RequestId = requestId,
                                ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                                Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                                Shift = model.Shift
                            };
                            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                            this._emailService.SendEmail(
                                ConfigurationManager.AppSettings["Email"],
                                item.Email,
                                string.Empty,
                                Resources.Messages.CheckInOutEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                                body);
                        }
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool LateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;

                int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
                int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;

                var requestId = this._employeeRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    model.Type,
                    false,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    model.ForDate,
                    lateInMins: lateInMins,
                    earlyOutMins: earlyOutMins);

                // check result
                if (errorCode == 0)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        // send email(s)
                        var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOut.Request.cshtml"));
                        foreach (var item in leaders)
                        {
                            var content = new
                            {
                                Leader = item.NickName,
                                Member = model.DisplayName,
                                Type = model.Type == (short)RequestTypes.LateIn
                                    ? "a " + Resources.Labels.LateIn.ToLower()
                                    : "an " + Resources.Labels.EarlyOut.ToLower(),
                                RequestId = requestId,
                                ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                                Minute = model.Minute,
                                Remark = model.Remark
                            };

                            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                            this._emailService.SendEmail(
                                ConfigurationManager.AppSettings["Email"],
                                item.Email,
                                string.Empty,
                                Resources.Messages.LateInEarlyOutEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                                body);
                        }
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;

                // half day leave
                if (model.HalfDayOff)
                {
                    if (model.TotalDays != (decimal)0.5)
                    {
                        modelState.AddModelError("TotalDays", Resources.Messages.InvalidTotalDays);
                        return false;
                    }

                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }
                else if (model.TotalDays % 1 != 0)
                {
                    modelState.AddModelError("TotalDays", Resources.Messages.InvalidTotalDays);
                    return false;
                }

                var requestId = this._employeeRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    (short)RequestTypes.Leave,
                    model.IsPlanned,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    fromDate: model.FromDate,
                    toDate: model.ToDate,
                    totalDays: model.TotalDays,
                    leaveTypeId: model.LeaveTypeId);

                // check result
                if (errorCode == 0)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        // send email(s)
                        var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/Leave.Request.cshtml"));
                        foreach (var item in leaders)
                        {
                            string subject = Resources.Messages.LeaveEmailSubject + " " + model.Employee;

                            string forDate = string.Empty;

                            if (model.TotalDays > 1)
                            {
                                subject += " from " + model.FromDate.ToDateMonthYearFormatedDate() + " to " + model.ToDate.ToDateMonthYearFormatedDate();
                                forDate = string.Format(
                                    MailContentLeave, 
                                    model.FromDate.ToDateMonthYearFormatedDate(), 
                                    model.ToDate.ToDateMonthYearFormatedDate(), 
                                    model.TotalDays);
                            }
                            else
                            {
                                subject += " on " + model.FromDate.ToDateMonthYearFormatedDate();
                                if (model.TotalDays == 1)
                                {
                                    forDate = string.Format(MailContentLeaveOneDay, model.FromDate.ToDateMonthYearFormatedDate());
                                }
                                else
                                {
                                    forDate = string.Format(
                                        MailContentLeaveHalfDay, 
                                        model.FromDate.ToDateMonthYearFormatedDate(), 
                                        model.FromDate.ToString("HH:mm"), 
                                        model.ToDate.ToString("HH:mm"));
                                }
                            }

                            var content = new
                            {
                                Leader = item.NickName,
                                Member = model.DisplayName,
                                RequestId = requestId,
                                ForDate = forDate,
                                Remark = model.Remark,
                                LeaveType = model.LeaveTypeName
                            };

                            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                            this._emailService.SendEmail(
                                ConfigurationManager.AppSettings["Email"],
                                item.Email,
                                string.Empty,
                                subject,
                                body);
                        }
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool ChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                int errorCode;
                string message;

                var requestId = this._employeeRepository.WorkingTimeRequest(
                    model.EmployeeId,
                    (short)RequestTypes.ChangeShift,
                    model.IsPlanned,
                    model.Remark,
                    DateTime.Now,
                    model.By,
                    out errorCode,
                    out message,
                    model.ForDate,
                    fromShift: model.FromShift,
                    toShift: model.ToShift);

                // check result
                if (errorCode == 0)
                {
                    // get leader(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    if (leaders == null || leaders.Count == 0)
                    {
                        string errorMessage;
                        this.Process(Commands.Approve.EnumToString(), requestId, model.EmployeeId, out errorMessage);
                    }
                    else
                    {
                        // send email(s)
                        var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShift.Request.cshtml"));
                        foreach (var item in leaders)
                        {
                            var content = new
                            {
                                Leader = item.NickName,
                                Member = model.DisplayName,
                                RequestId = requestId,
                                ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                                FromShift = model.FromShiftName,
                                ToShift = model.ToShiftName,
                                Remark = model.Remark
                            };
                            var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                            this._emailService.SendEmail(
                                ConfigurationManager.AppSettings["Email"],
                                item.Email,
                                string.Empty,
                                Resources.Messages.ChangeShiftEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                                body);
                        }
                    }

                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, message);
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

                var updated = this._employeeRepository.UpdateRequest(
                    model.Id,
                    false,
                    null,
                    model.EmployeeId,
                    model.ForDate,
                    DateTime.Now);

                if (updated)
                {
                    // send email(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOut.Request.cshtml"));
                    foreach (var item in leaders)
                    {
                        var content = new
                        {
                            Leader = item.NickName,
                            Member = model.DisplayName,
                            Type = model.Type == (short)RequestTypes.CheckIn
                                ? RequestTypes.CheckIn.EnumToString().ToLower()
                                : RequestTypes.CheckOut.EnumToString().ToLower(),
                            RequestId = model.Id,
                            ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                            Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                            Shift = model.Shift
                        };

                        var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                        this._emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            item.Email,
                            string.Empty,
                            Resources.Messages.UpdatedCheckInOutEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                            body);
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool UpdateLateInEarlyOut(ModelStateDictionary modelState, LateInEarlyOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                int? lateInMins = model.Type == (short)RequestTypes.LateIn ? (int?)model.Minute : null;
                int? earlyOutMins = model.Type == (short)RequestTypes.EarlyOut ? (int?)model.Minute : null;

                var updated = this._employeeRepository.UpdateRequest(
                    model.Id,
                    false,
                    model.Remark,
                    model.By,
                    model.ForDate,
                    DateTime.Now,
                    lateInMins: lateInMins,
                    earlyOutMins: earlyOutMins);

                if (updated)
                {
                    // send email(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOut.Request.cshtml"));
                    foreach (var item in leaders)
                    {
                        var content = new
                        {
                            Leader = item.NickName,
                            Member = model.DisplayName,
                            Type = model.Type == (short)RequestTypes.LateIn
                                ? "a " + Resources.Labels.LateIn.ToLower()
                                : "an " + Resources.Labels.EarlyOut.ToLower(),
                            RequestId = model.Id,
                            ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                            Minute = model.Minute,
                            Remark = model.Remark
                        };

                        var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                        this._emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            item.Email,
                            string.Empty,
                            Resources.Messages.UpdatedLateInEarlyOutEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                            body);
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                // half day leave
                if (model.HalfDayOff)
                {
                    if (model.TotalDays != (decimal)0.5)
                    {
                        modelState.AddModelError("TotalDays", Resources.Messages.InvalidTotalDays);
                        return false;
                    }

                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }
                else if (model.TotalDays % 1 != 0)
                {
                    modelState.AddModelError("TotalDays", Resources.Messages.InvalidTotalDays);
                    return false;
                }

                var updated = this._employeeRepository.UpdateRequest(
                    model.Id,
                    model.IsPlanned,
                    model.Remark,
                    model.EmployeeId,
                    model.FromDate,
                    DateTime.Now,
                    toDate: model.ToDate,
                    leaveTypeId: model.LeaveTypeId,
                    totalDays: model.TotalDays);

                if (updated)
                {
                    // send email(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/Leave.Request.cshtml"));
                    foreach (var item in leaders)
                    {
                        string subject = Resources.Messages.UpdatedLeaveEmailSubject + " " + model.Employee;

                        string forDate = string.Empty;

                        if (model.TotalDays > 1)
                        {
                            subject += " from " + model.FromDate.ToDateMonthYearFormatedDate() + " to " + model.ToDate.ToDateMonthYearFormatedDate();
                            forDate = string.Format(
                                MailContentLeave, 
                                model.FromDate.ToDateMonthYearFormatedDate(),
                                model.ToDate.ToDateMonthYearFormatedDate(), 
                                model.TotalDays);
                        }
                        else
                        {
                            subject += " on " + model.FromDate.ToDateMonthYearFormatedDate();
                            if (model.TotalDays == 1)
                            {
                                forDate = string.Format(MailContentLeaveOneDay, model.FromDate.ToDateMonthYearFormatedDate());
                            }
                            else
                            {
                                forDate = string.Format(
                                    MailContentLeaveHalfDay, 
                                    model.FromDate.ToDateMonthYearFormatedDate(),
                                    model.FromDate.ToString("HH:mm"), 
                                    model.ToDate.ToString("HH:mm"));
                            }
                        }

                        var content = new
                        {
                            Leader = item.NickName,
                            Member = model.DisplayName,
                            RequestId = model.Id,
                            ForDate = forDate,
                            Remark = model.Remark,
                            LeaveType = model.LeaveTypeName
                        };

                        var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                        this._emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            item.Email,
                            string.Empty,
                            subject,
                            body);
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        public bool UpdateChangeShift(ModelStateDictionary modelState, ChangeShiftViewModel model)
        {
            if (modelState.IsValid)
            {
                var updated = this._employeeRepository.UpdateRequest(
                    model.Id,
                    model.IsPlanned,
                    model.Remark,
                    model.By,
                    model.ForDate,
                    DateTime.Now,
                    fromShift: model.FromShift,
                    toShift: model.ToShift);

                if (updated)
                {
                    // send email(s)
                    var leaders = this._employeeRepository.GetLeaders(model.EmployeeId);

                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShift.Request.cshtml"));
                    foreach (var item in leaders)
                    {
                        var content = new
                        {
                            Leader = item.NickName,
                            Member = model.DisplayName,
                            RequestId = model.Id,
                            ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                            FromShift = model.FromShiftName,
                            ToShift = model.ToShiftName,
                            Remark = model.Remark
                        };

                        var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                        this._emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            item.Email,
                            string.Empty,
                            Resources.Messages.UpdatedChangeShiftEmailSubject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                            body);
                    }
                }

                return updated;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }
        
        /// <summary>
        /// Approve, disapprove or reject a request, notification email will be sent also
        /// </summary>
        /// <param name="command">Action on this request.</param>
        /// <param name="id">Request id.</param>
        /// <param name="processBy">Process by.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Return boolean.</returns>
        public bool Process(string command, long id, int processBy, out string errorMessage)
        {
            var request = this._employeeRepository.GetWorkingTimeRequest(id);
            var employee = this._employeeRepository.Get(request.EmpId);

            var processUser = this._employeeRepository.Get(processBy);

            Commands processCommand;
            switch (command)
            {
                case "Approve":
                    processCommand = Commands.Approve;
                    break;
                case "Disapprove":
                    processCommand = Commands.Disapprove;
                    break;
                case "Reject":
                    processCommand = Commands.Reject;
                    break;
                default:
                    processCommand = Commands.Approve;
                    break;
            }

            string message;
            int code = this._employeeRepository.ProcessRequest(
                processCommand, id, processBy, DateTime.Now, out message);

            if (code == 0)
            {
                //var allShifts = this._shiftService.GetAll();
                string forDate = string.Empty;

                if (request.TotalDays > 1)
                {
                    forDate = " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " + request.ToDate.ToDateMonthYearFormatedDate();
                }
                else
                {
                    forDate = " on " + request.FromDate.ToDateMonthYearFormatedDate();
                }

                // send email to the person who made this request
                var bodyContent = new
                {
                    Employee = employee.NickName,
                    RequestId = request.Id,
                    Action = command == Commands.Reject.EnumToString()
                    ? Resources.Labels.Rejected
                    : (command == Commands.Approve.EnumToString()
                        ? Resources.Labels.Approved
                        : Resources.Labels.Disapproved),
                    ForDate = forDate,
                    Request = request.RequestType.ToText(),
                    User = processUser.NickName,
                    DisplayNameUser = processUser.FullName
                };

                var body = Utilities.ReadTextFile(
                    HttpContext.Current.Server.MapPath("~/app_data/emails/process.request.cshtml"));
                //body = RazorEngine.Razor.Parse(body, bodyContent);
                body = string.Format(body, bodyContent.Employee, bodyContent.Request, bodyContent.ForDate, bodyContent.Action, bodyContent.User, bodyContent.DisplayNameUser);

                var subject = string.Format(
                    Resources.Messages.ProcessSubject,
                    request.RequestType.ToText(),
                    forDate,
                    bodyContent.Action,
                    processUser.NickName);

                this._emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    employee.Email,
                    string.Empty,
                    subject,
                    body);

                this.SendToRelatedPeople(processUser, request, employee, processCommand);
            }
            else
            {
                errorMessage = message;
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private static string GetMessageDateforLeaveRequest(DateTime fromDate, DateTime toDate)
        {
            string result = string.Empty;
            if ((toDate - fromDate).Days > 0)
            {
                if (fromDate.Year == toDate.Year)
                {
                    if (fromDate.Month != toDate.Month)
                    {
                        result = fromDate.ToString("dd MMM") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                    else
                    {
                        result = fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }
                }
                else
                {
                    result = fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
                }
            }
            else
            {
                result = fromDate.ToDateMonthYearFormatedDate();
            }

            return result;
        }

        private IEnumerable<Employee> AddProjectManagerIntoList(IEnumerable<Employee> employees, int employeeId)
        {
            var teamCodes = ConfigurationManager.AppSettings["ProjectManagerTeams"].ToString();
            var ignoreTeams = ConfigurationManager.AppSettings["IgnoreTeams"].ToString();
            var projectManagers = this._employeeRepository.GetEmployeeByTeams(employeeId, teamCodes, ignoreTeams);

            if (employees != null && projectManagers != null)
            {
                employees = employees.Union(projectManagers);
            }
            else if (employees != null)
            {
                employees = projectManagers;
            }
            
            return employees;
        }

        private void SendToRelatedPeople(Employee processUser, WorkingTimeRequest request, Employee employee, Commands processCommand)
        {
            if (processCommand == Commands.Approve)
            {
                // send notification email to employee(s) who has related working
                var employees = this._employeeRepository.GetEmployeeForRelatedTeam(employee.Id)
                    .Where(i => i.Id != processUser.Id && i.Id != request.EmpId);

                if (request.RequestType == (int)RequestTypes.ChangeShift || request.RequestType == (int)RequestTypes.Leave)
                {
                    employees = AddProjectManagerIntoList(employees, employee.Id);
                }

                var sendCc = string.Join(",", employees.Select(t => t.Email));
                //var sendCc = string.Empty;

                switch (request.RequestType)
                {
                    case 1:
                        var subjectCheckIn = string.Format(
                            Resources.Messages.ApprovalNotificationCheckInSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        CheckInOutRelatedMail(employee, processUser, request, subjectCheckIn, sendCc);
                        break;
                    case 2:
                        var subjectCheckOut = string.Format(
                            Resources.Messages.ApprovalNotificationCheckOutSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        CheckInOutRelatedMail(employee, processUser, request, subjectCheckOut, sendCc);
                        break;
                    case 3:
                        var subjectLateIn = string.Format(
                            Resources.Messages.ApprovalNotificationLateInSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        LateInEarlyOutRelatedMail(employee, processUser, request, subjectLateIn, sendCc, isLateIn: true);
                        break;
                    case 4:
                        var subjectEarlyOut = string.Format(
                            Resources.Messages.ApprovalNotificationEarlyOutSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        LateInEarlyOutRelatedMail(employee, processUser, request, subjectEarlyOut, sendCc, isLateIn: false);
                        break;
                    case 5:
                        var subjectChangeShift = string.Format(
                            Resources.Messages.ApprovalNotificationChangeShiftSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        ChangeShiftRelatedMail(employee, processUser, request, subjectChangeShift, sendCc);
                        break;
                    case 6:
                        var subjectLeave = string.Format(
                            Resources.Messages.ApprovalNotificationLeaveSubject, 
                            employee.NickName, 
                            processUser.NickName);
                        LeaveRelatedMail(employee, processUser, request, subjectLeave, sendCc);
                        break;
                }
            }
        }

        private void CheckInOutRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc)
        {
            var shift = this._shiftService.GetByDate(employee.Id, request.FromDate);

            var body = Utilities.ReadTextFile(
                HttpContext.Current.Server.MapPath("~/app_data/emails/CheckInOutApprovalNotification.cshtml"));
            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                Time = request.FromDate.ToString("HH:mm"),
                Shift = shift != null ? shift.Name : string.Empty
            };

            // body = RazorEngine.Razor.Parse(body, bodyContent);
            body = string.Format(body, bodyContent.User, bodyContent.Employee, bodyContent.Request, bodyContent.ForDate, bodyContent.Shift, bodyContent.Time);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        private void LateInEarlyOutRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc, bool isLateIn)
        {
            var body = Utilities.ReadTextFile(
                HttpContext.Current.Server.MapPath("~/app_data/emails/LateInEarlyOutApprovalNotification.cshtml"));
            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType == (short)RequestTypes.LateIn
                                ? "a " + Resources.Labels.LateIn.ToLower()
                                : "an " + Resources.Labels.EarlyOut.ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                Minute = isLateIn ? request.LateInMins : request.EarlyOutMins,
                Remark = request.Remark
            };

            // body = RazorEngine.Razor.Parse(body, bodyContent);
            body = string.Format(body, bodyContent.User, bodyContent.Employee, bodyContent.Request, bodyContent.ForDate, bodyContent.Minute, bodyContent.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        private void ChangeShiftRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc)
        {
            var allShifts = this._shiftService.GetAll();
            var fromShift = allShifts.Where(t => t.Id == (request.FromShift.HasValue ? request.FromShift.Value : 0)).SingleOrDefault();
            var toShift = allShifts.Where(t => t.Id == (request.ToShift.HasValue ? request.ToShift.Value : 0)).SingleOrDefault();

            var body = Utilities.ReadTextFile(
                HttpContext.Current.Server.MapPath("~/app_data/emails/ChangeShiftApprovalNotification.cshtml"));
            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = request.FromDate.ToDateMonthYearFormatedDate(),
                FromShift = fromShift != null ? fromShift.Name : string.Empty,
                ToShift = toShift != null ? toShift.Name : string.Empty,
                Remark = request.Remark
            };

            //body = RazorEngine.Razor.Parse(body, bodyContent);
            body = string.Format(body, bodyContent.User, bodyContent.Employee, bodyContent.Request, bodyContent.ForDate, bodyContent.FromShift, bodyContent.ToShift, bodyContent.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }

        private void LeaveRelatedMail(Employee employee, Employee processUser, WorkingTimeRequest request, string subject, string sendCc)
        {
            var allLeaveTypes = _leaveTypeService.GetAll();
            var leaveType = allLeaveTypes.Where(t => t.Id == (request.LeaveTypeID.HasValue ? request.LeaveTypeID.Value : 0)).SingleOrDefault();
            var body = Utilities.ReadTextFile(
                HttpContext.Current.Server.MapPath("~/app_data/emails/LeaveApprovalNotification.cshtml"));

            var forDate = string.Empty;
            if (request.TotalDays > 1)
            {
                forDate = string.Format(
                    MailContentLeave, 
                    request.FromDate.ToDateMonthYearFormatedDate(),
                    request.ToDate.ToDateMonthYearFormatedDate(), 
                    request.TotalDays);
            }
            else
            {
                if (request.TotalDays == 1)
                {
                    forDate = string.Format(MailContentLeaveOneDay, request.FromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    forDate = string.Format(
                        MailContentLeaveHalfDay, 
                        request.FromDate.ToDateMonthYearFormatedDate(),
                        request.FromDate.ToString("HH:mm"),
                        request.ToDate.HasValue ? request.ToDate.Value.ToString("HH:mm") : request.FromDate.ToString("HH:mm"));
                }
            }

            var bodyContent = new
            {
                User = processUser.NickName,
                Employee = string.Format("{0} ({1})", employee.NickName, employee.FullName),
                Request = request.RequestType.ToText().ToLower(),
                ForDate = forDate,
                Remark = request.Remark,
                LeaveType = leaveType != null ? leaveType.Name : string.Empty
            };

            //body = RazorEngine.Razor.Parse(body, bodyContent);
            body = string.Format(
                body, 
                bodyContent.User, 
                bodyContent.Employee, 
                bodyContent.Request, 
                bodyContent.ForDate, 
                bodyContent.LeaveType, 
                bodyContent.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                processUser.Email,
                sendCc,
                subject,
                body);
        }
    }
}