// --------------------------------------------------------------------------
// <copyright file="RequestService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.10">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Request service.
    /// </summary>
    public class RequestService : BaseService, IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public RequestService(
            IRequestRepository requestRepository = null,
            IEmployeeRepository employeeRepository = null, 
            IEmailService emailService = null)
        {
            this._requestRepository = requestRepository ?? new RequestRepository();
            this._employeeRepository = employeeRepository ?? new EmployeeRepository();
            this._emailService = emailService ?? new EmailService();
        }

        public static string ToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            switch (request.RequestType.Id)
            {
                case (int)RequestType.Leave:
                    if (request.TotalDays > 1)
                    {
                        msg.AppendFormat(Constants.MsgLeave, Convert.ToInt32(request.TotalDays), GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    }
                    else if (request.TotalDays == 1)
                    {
                        msg.AppendFormat(Constants.MsgLeaveOneDay, GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
                    }
                    else
                    {
                        // 0.5 day
                        msg.AppendFormat(
                            Constants.MsgLeaveHalfDay, 
                            GetMessageDateforLeaveRequest(request.FromDate, request.ToDate), 
                            request.FromDate.ToString("HH:mm"), 
                            request.ToDate.ToString("HH:mm"));
                    }

                    break;

                case (int)RequestType.CheckIn:
                case (int)RequestType.CheckOut:
                    msg.AppendFormat(Constants.MsgCheckInOut, request.FromDate.ToString("HH:mm"), request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;

                case (int)RequestType.LateIn:
                    msg.AppendFormat(Constants.MsgLateIn, request.TotalMins, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestType.EarlyOut:
                    msg.AppendFormat(Constants.MsgEarlyOut, request.TotalMins, request.FromShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
                    break;
                case (int)RequestType.ChangeShift:
                    msg.AppendFormat(Constants.MsgChangeShift, request.FromShift.Name, request.ToShift.Name, request.FromDate.ToDateMonthYearFormatedDate());
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
                var criteria = new InsertRequestCriteria()
                {
                    EmpId = model.EmployeeId,
                    RequestType = model.CheckInOutType,
                    IsPlanned = false,
                    Remark = null,
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.By,
                    FromDate = model.ForDate,
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = Repository.Insert(criteria, hasGetOutputParams: true);
                model.Id = criteria.Id;
                
                if (result.Code == 0)
                {
                    string templateEmailPath = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForCheckInOut);
                    var taskSendMail = new Task(() => SendMailForCheckInOut(model, Messages.CheckInOutEmailSubject, templateEmailPath));
                    taskSendMail.Start();
                    return true;
                }
                else if (result.Code != -1)
                {
                    modelState.AddModelError(string.Empty, result.Message);
                    return false;
                }

                return true;
            }
            else
            {
                modelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return false;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public bool Leave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                if (model.HalfDayOff)
                {
                    if (model.TotalDays != (decimal)0.5)
                    {
                        modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                        return false;
                    }

                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }
                else if (model.TotalDays % 1 != 0)
                {
                    modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                    return false;
                }

                var criteria = new InsertRequestCriteria()
                {
                    EmpId = model.EmployeeId,
                    RequestType = (short)RequestType.Leave,
                    IsPlanned = model.IsPlanned,
                    Remark = model.Remark,
                    CreatedDate = DateTime.Now,
                    CreatedBy = model.By,
                    FromDate = model.FromDate,
                    FromShift = null,
                    ToDate = model.ToDate,
                    ToShift = null,
                    LeaveTypeId = model.LeaveTypeId,
                    TotalDays = model.TotalDays,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = Repository.Insert(criteria, hasGetOutputParams: true);
                model.Id = criteria.Id;

                if (result.Code == 0)
                {
                    string templateEmailPath = model.LeaveTypeId == 26 || model.TotalDays >= 5 
                        ? HttpContext.Current.Server.MapPath("~/app_data/emails/UnpaidLeave.Request.cshtml")
                        : HttpContext.Current.Server.MapPath("~/app_data/emails/Leave.Request.cshtml");
                    var taskSendMail = new Task(() => SendMailForLeave(model, Messages.LeaveEmailSubject + " " + model.Employee, templateEmailPath));
                    taskSendMail.Start();
                    return true;
                }
                else if (result.Code != -1)
                {
                    modelState.AddModelError(string.Empty, result.Message);
                    return false;
                }

                return true;
            }
            else
            {
                modelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return false;
        }

        public bool UpdateCheckInOut(ModelStateDictionary modelState, CheckInOutRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                model.ForDate = model.ForDate + new TimeSpan(model.Hour, model.Minute, 0);

                var criteria = new UpdateRequestCriteria()
                {
                    Id = model.Id,
                    IsPlanned = false,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.ForDate,
                    ModifiedDate = DateTime.Now,
                    FromShift = null,
                    ToDate = null,
                    ToShift = null,
                    LeaveTypeId = null,
                    TotalDays = null,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = Repository.Insert(criteria, hasGetOutputParams: true);

                if (result.Code == 0)
                {
                    string templateEmailPath = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForCheckInOut);
                    var taskSendMail = new Task(() => SendMailForCheckInOut(model, Messages.UpdatedCheckInOutEmailSubject, templateEmailPath));
                    taskSendMail.Start();
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, result.Message);
                    return false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return false;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public bool UpdateLeave(ModelStateDictionary modelState, LeaveRequestViewModel model)
        {
            if (modelState.IsValid)
            {
                // half day leave
                if (model.HalfDayOff)
                {
                    if (model.TotalDays != (decimal)0.5)
                    {
                        modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                        return false;
                    }

                    model.FromDate = Utilities.AddTimeToDate(model.FromDate, model.FromTime);
                    model.ToDate = Utilities.AddTimeToDate(model.ToDate, model.ToTime);
                }
                else if (model.TotalDays % 1 != 0)
                {
                    modelState.AddModelError("TotalDays", Messages.InvalidTotalDays);
                    return false;
                }

                var criteria = new UpdateRequestCriteria()
                {
                    Id = model.Id,
                    IsPlanned = model.IsPlanned,
                    Remark = model.Remark,
                    ModifiedBy = model.EmployeeId,
                    FromDate = model.FromDate,
                    ModifiedDate = DateTime.Now,
                    FromShift = null,
                    ToDate = model.ToDate,
                    ToShift = null,
                    LeaveTypeId = model.LeaveTypeId,
                    TotalDays = model.TotalDays,
                    LateInMins = null,
                    EarlyOutMins = null
                };

                CommandResult result = Repository.Insert(criteria, hasGetOutputParams: true);

                if (result.Code == 0)
                {
                    string templateEmailPath = model.LeaveTypeId == 26 || model.TotalDays >= 5
                        ? HttpContext.Current.Server.MapPath("~/app_data/emails/UnpaidLeave.Request.cshtml")
                        : HttpContext.Current.Server.MapPath("~/app_data/emails/Leave.Request.cshtml");
                    var taskSendMail = new Task(() => SendMailForLeave(model, Messages.UpdatedLeaveEmailSubject + " " + model.Employee, templateEmailPath));
                    taskSendMail.Start();
                    return true;
                }
                else
                {
                    modelState.AddModelError(string.Empty, result.Message);
                    return false;
                }
            }
            else
            {
                modelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return false;
        }

        public CommandResult Process(string command, long id, int processBy)
        {
            ICriteria processRequestCriteria;
            switch (command)
            {
                case "Disapprove":
                    processRequestCriteria = new DisapproveRequestCriteria
                    {
                        RequestId = id,
                        ModifiedBy = processBy,
                        ModifiedDate = DateTime.Now
                    };
                    break;
                case "Reject":
                    processRequestCriteria = new RejectRequestCriteria
                    {
                        RequestId = id,
                        ProcessedBy = processBy,
                        ProcessedDate = DateTime.Now
                    };
                    break;
                default:
                    processRequestCriteria = new ApproveRequestCriteria
                    {
                        RequestId = id,
                        ApprovedBy = processBy,
                        ApprovedDate = DateTime.Now
                    };
                    break;
            }

            CommandResult result = Repository.Update(processRequestCriteria);

            if (result.Code == 0)
            {
                string templateEmailToRequester = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForRequester);
                string templateLeaveEmailToRelated = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForLeaveApproved);
                string templateInOutEmailToRelated = HttpContext.Current.Server.MapPath(Constants.EmailTemplatePathForCheckInOutApproved);
                var taskSendMail = new Task(() => SendMailForProcessRequest(id, command, templateEmailToRequester, templateLeaveEmailToRelated, templateInOutEmailToRelated));
                taskSendMail.Start();
            }

            return result;
        }

        public IList<RequestEntity> GetRequest(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria.IsValid())
            {
                entityList = this._requestRepository.GetRequest(criteria);
            }

            return entityList;
        }

        public RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            RequestEntity request = new RequestEntity();
            if (criteria.IsValid())
            {
                request = this._requestRepository.GetRequestEntity(criteria);
            }

            return request;
        }

        public bool DeleteRequest(DeleteRequestCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                Repository.Delete(criteria);
                return true;
            }

            return false;
        }

        public IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<RequestEntity> requests = new List<RequestEntity>();
            if (criteria.IsValid())
            {
                requests = this._requestRepository.GetRequest(criteria);
            }

            return requests;
        }

        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            return _requestRepository.GetNumberRequestInMonth(criteria);
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

        private void SendMailForCheckInOut(CheckInOutRequestViewModel model, string subject, string templateEmailPath)
        {
            IList<Employee> leaders = GetLeaderForCheckInOut(model);
            if (leaders != null && leaders.Any())
            {
                // send email(s)
                var bodyTemplate = Utilities.ReadTextFile(templateEmailPath);
                foreach (var item in leaders)
                {
                    var content = new
                    {
                        Leader = item.NickName,
                        Member = model.DisplayName,
                        Type = model.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower()
                            : RequestType.CheckOut.EnumToString().ToLower(),
                        RequestId = model.Id,
                        ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                        Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                        Shift = model.Shift
                    };

                    var body = string.Format(bodyTemplate, content.Leader, content.Member, content.Type, content.ForDate, content.Shift, content.Time);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        item.Email,
                        string.Empty,
                        subject + " " + model.Employee + " on " + model.ForDate.ToDateMonthYearFormatedDate(),
                        body);
                }
            }
        }

        private IList<Employee> GetLeaderForCheckInOut(CheckInOutRequestViewModel model)
        {
            IList<Employee> leaders = null;
            if (string.Equals(model.Shift, "A2") 
                && (model.ForDate.DayOfWeek == DayOfWeek.Saturday || model.ForDate.DayOfWeek == DayOfWeek.Sunday))
            {
                var getLeaderCriteria = new GetLeaderForStanbyTeamCriteria();
                leaders = this._employeeRepository.GetLeaders(getLeaderCriteria);
            }
            else
            {
                var criteria = new GetLeadersOfEmployeeCriteria()
                {
                    Id = model.EmployeeId
                };
                leaders = this._employeeRepository.GetLeaders(criteria);
            }
            
            return leaders;
        }

        private void SendMailForLeave(LeaveRequestViewModel model, string subject, string templateEmailPath)
        {
            var body = string.Empty;
            var bodyTemplate = string.Empty;
            IList<Employee> managers = null;
            IList<Employee> leaders = null;
            var criteria = new GetLeadersOfEmployeeCriteria() { Id = model.EmployeeId };
            leaders = this._employeeRepository.GetLeaders(criteria);

            subject = model.TotalDays > 1 
                ? subject + " from " + model.FromDate.ToDateMonthYearFormatedDate() + " to " + model.ToDate.ToDateMonthYearFormatedDate()
                : subject + " on " + model.FromDate.ToDateMonthYearFormatedDate();

            bodyTemplate = Utilities.ReadTextFile(templateEmailPath);

            var forDate = GetDateStringForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);
            if (model.LeaveTypeId == 26 || model.TotalDays >= 5)
            {
                var getManagementsCriteria = new GetManagementsCriteria();
                managers = this._employeeRepository.GetManagements(getManagementsCriteria);
                
                if (managers != null && managers.Any())
                {
                    body = string.Format(bodyTemplate, model.DisplayName, forDate, model.LeaveTypeName, model.Remark);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        string.Join(";", managers.Select(t => t.Email)),
                        string.Join(";", leaders.Select(t => t.Email)),
                        subject,
                        body);
                }
            }
            else
            {
                if (leaders != null && leaders.Any())
                {
                    body = string.Format(bodyTemplate, leaders.First().NickName, model.DisplayName, forDate, model.LeaveTypeName, model.Remark);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        string.Join(";", leaders.Select(t => t.Email)),
                        string.Empty,
                        subject,
                        body);
                }
            }
        }

        private string GetDateStringForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            string forDate = string.Empty;

            if (totalDays > 1)
            {
                forDate = string.Format(Constants.MailContentLeave, fromDate.ToDateMonthYearFormatedDate(), toDate.ToDateMonthYearFormatedDate(), totalDays);
            }
            else
            {
                if (totalDays == 1)
                {
                    forDate = string.Format(Constants.MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
                }
                else
                {
                    forDate = string.Format(
                        Constants.MailContentLeaveHalfDay,
                        fromDate.ToDateMonthYearFormatedDate(),
                        fromDate.ToString("HH:mm"),
                        toDate.ToString("HH:mm"));
                }
            }

            return forDate;
        }

        private IEnumerable<Employee> AddProjectManagerIntoList(IEnumerable<Employee> employees, int employeeId)
        {
            var getEmployeeByTeamsCriteria = new GetEmployeeByTeamsCriteria
            {
                EmployeeId = employeeId,
                TeamCodes = AppSetting.ProjectManagerTeams,
                IgnoreTeams = AppSetting.IgnoreTeams
            };
            var projectManagers = this._employeeRepository.GetEmployeeByTeams(getEmployeeByTeamsCriteria);

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

        private void SendMailForProcessRequest(
            long id, 
            string command, 
            string templateEmailToRequesterPath, 
            string templateLeaveEmailToRelatedPath, 
            string templateInOutEmailToRelatedPath)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria() { Id = id };
            var request = this._requestRepository.GetRequestEntity(getRequestByIdCriteria);

            SendMailToRequester(request, command, templateEmailToRequesterPath);

            if (command == Command.Approve.EnumToString())
            {
                this.SendMailToRelatedPeople(request, templateLeaveEmailToRelatedPath, templateInOutEmailToRelatedPath);
            }
        }

        private void SendMailToRequester(RequestEntity request, string command, string templateEmailToRequesterPath)
        {
            string forDate = string.Empty;
            if (request.TotalDays > 1)
            {
                forDate = " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " + request.ToDate.ToDateMonthYearFormatedDate();
            }
            else
            {
                forDate = " on " + request.FromDate.ToDateMonthYearFormatedDate();
            }

            var body = Utilities.ReadTextFile(templateEmailToRequesterPath);
            var action = command == Command.Reject.EnumToString()
                    ? Resources.Labels.Rejected
                    : (command == Command.Approve.EnumToString() ? Labels.Approved : Labels.Disapproved);
            body = string.Format(
                body,
                request.Requester.NickName,
                request.RequestType.Name,
                forDate,
                action,
                request.ModifiedBy.NickName,
                request.ModifiedBy.FullName);

            var subject = string.Format(Messages.ProcessSubject, request.RequestType.Name, forDate, action, request.ModifiedBy.NickName);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.Requester.Email,
                string.Empty,
                subject,
                body);
        }

        private void SendMailToRelatedPeople(RequestEntity request, string templateLeaveEmailToRelatedPath, string templateInOutEmailToRelatedPath)
        {
            var criteria = new GetEmployeeForRelatedTeamCriteria() { EmpId = request.Requester.Id };
            var employees = this._employeeRepository.GetEmployeeForRelatedTeam(criteria)
                .Where(i => i.Id != request.ModifiedBy.Id && i.Id != request.Requester.Id);

            if (request.RequestType.Id == (int)RequestType.Leave)
            {
                employees = AddProjectManagerIntoList(employees, request.Requester.Id);
            }

            var sendCc = string.Join(",", employees.Select(t => t.Email));

            switch (request.RequestType.Id)
            {
                case 1:
                case 2:
                    CheckInOutRelatedMail(request, sendCc, templateInOutEmailToRelatedPath);
                    break;
                case 6:
                    LeaveRelatedMail(request, sendCc, templateLeaveEmailToRelatedPath);
                    break;
            }
        }

        private void CheckInOutRelatedMail(RequestEntity request, string sendCc, string templateInOutEmailToRelatedPath)
        {
            var subject = request.RequestType.Id == (int)RequestType.CheckIn 
                ? string.Format(
                    Messages.ApprovalNotificationCheckInSubject,
                    request.Requester.NickName,
                    request.ModifiedBy.NickName) 
                : string.Format(
                    Messages.ApprovalNotificationCheckOutSubject,
                    request.Requester.NickName,
                    request.ModifiedBy.NickName);

            var body = Utilities.ReadTextFile(templateInOutEmailToRelatedPath);

            body = string.Format(
                body,
                request.ModifiedBy.NickName,
                string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName),
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name, 
                request.FromDate.ToString("HH:mm"));

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.ModifiedBy.Email,
                sendCc,
                subject,
                body);
        }

        private void LeaveRelatedMail(RequestEntity request, string sendCc, string templateLeaveEmailToRelatedPath)
        {
            var subject = string.Format(
                Messages.ApprovalNotificationLeaveSubject,
                request.Requester.NickName,
                request.ModifiedBy.NickName);
            var body = Utilities.ReadTextFile(templateLeaveEmailToRelatedPath);
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);

            body = string.Format(
                body,
                request.ModifiedBy.NickName,
                string.Format("{0} ({1})", request.Requester.NickName, request.Requester.FullName),
                Labels.Leave,
                forDate, 
                request.LeaveType.Name, 
                request.Remark);

            this._emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.ModifiedBy.Email,
                sendCc,
                subject,
                body);
        }
    }
}