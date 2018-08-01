namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request;
    using Data.Repository;
    using Interfaces;
    using Models;
    using Models.Request;
    using Repositories;
    using Repositories.Interfaces;
    using Resources;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class RequestService : BaseService, IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IEmailService _emailService;
        private readonly ICommandService _commandService;
        private readonly INexbotService _nexbotService;

        public RequestService()
            : this(null, null, null, null)
        {
        }

        public RequestService(
            IRequestRepository requestRepository,
            IEmailService emailService,
            ICommandService commandService,
            INexbotService nexbotService)
        {
            _requestRepository = requestRepository ?? new RequestRepository();
            _emailService = emailService ?? new EmailService();
            _commandService = commandService ?? new CommandService();
            _nexbotService = nexbotService ?? new NexbotService();
        }

        private static string GetMessageDateforLeaveRequest(DateTime fromDate, DateTime toDate)
        {
            var result = (toDate - fromDate).Days > 0
                ? (fromDate.Year == toDate.Year
                    ? (fromDate.Month != toDate.Month
                        ? fromDate.ToString("dd MMM") + " - " + toDate.ToDateMonthYearFormatedDate()
                        : fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate())
                    : fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate())
                : fromDate.ToDateMonthYearFormatedDate();

            return result;
        }

        public static string ConvertToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            if (request != null)
            {
                switch (request.RequestType.Id)
                {
                    case (int)RequestType.Leave:
                        msg = CreateLeavemessage(request);
                        break;

                    case (int)RequestType.CheckIn:
                    case (int)RequestType.CheckOut:
                        msg.AppendFormat(
                            Constants.MsgCheckInOut,
                            request.FromDate.ToString("HH:mm"),
                            request.FromShift.Name,
                            request.FromDate.ToDateMonthYearFormatedDate());
                        break;

                    case (int)RequestType.LateIn:
                        msg.AppendFormat(
                            Constants.MsgLateIn,
                            request.TotalMins,
                            request.FromShift.Name,
                            request.FromDate.ToDateMonthYearFormatedDate());
                        break;

                    case (int)RequestType.EarlyOut:
                        msg.AppendFormat(
                            Constants.MsgEarlyOut,
                            request.TotalMins,
                            request.FromShift.Name,
                            request.FromDate.ToDateMonthYearFormatedDate());
                        break;

                    case (int)RequestType.ChangeShift:
                        msg.AppendFormat(
                            Constants.MsgChangeShift,
                            request.FromShift.Name,
                            request.ToShift.Name,
                            request.FromDate.ToDateMonthYearFormatedDate());
                        break;

                    default:
                        msg = new StringBuilder();
                        break;
                }
            }

            return msg.ToString();
        }

        private static StringBuilder CreateLeavemessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            if (request.TotalDays > 1)
            {
                msg.AppendFormat(
                    Constants.MsgLeave,
                    Convert.ToInt32(request.TotalDays),
                    GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
            }
            else if (request.TotalDays == 1)
            {
                msg.AppendFormat(
                    Constants.MsgLeaveOneDay,
                    GetMessageDateforLeaveRequest(request.FromDate, request.ToDate));
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

            return msg;
        }

        public CommandResult Process(string command, long id, int processBy)
        {
            ICriteria processRequestCriteria = GetProcessRequestCriteria(command, id, processBy);

            CommandResult result = _commandService.Update(processRequestCriteria);

            if (result.IsSuccessful)
            {
                string templateEmailToRequester = HttpContext.Current.Server
                    .MapPath(Constants.EmailTemplatePathForRequester);
                string templateLeaveEmailToRelated = HttpContext.Current.Server
                    .MapPath(Constants.EmailTemplatePathForLeaveApproved);
                string templateInOutEmailToRelated = HttpContext.Current.Server
                    .MapPath(Constants.EmailTemplatePathForCheckInOutApproved);

                Task.Factory.StartNew(() =>
                {
                    SendMailForProcessRequest(
                        id,
                        command,
                        templateEmailToRequester,
                        templateLeaveEmailToRelated,
                        templateInOutEmailToRelated);

                    _nexbotService.SendMessageProcessRequest(GetRequestEntity(id), command);
                });
            }

            return result;
        }

        private static ICriteria GetProcessRequestCriteria(string command, long id, int processBy)
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

            return processRequestCriteria;
        }

        public IList<RequestEntity> GetRequest(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria != null && criteria.IsValid())
            {
                entityList = _requestRepository.GetRequest(criteria);
            }

            return entityList ?? new List<RequestEntity>();
        }

        public RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria)
        {
            RequestEntity request = new RequestEntity();
            if (criteria != null && criteria.IsValid())
            {
                request = _requestRepository.GetRequestEntity(criteria);
            }

            return request;
        }

        public IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<RequestEntity> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = _requestRepository.GetRequest(criteria);
            }

            return requests ?? new List<RequestEntity>();
        }

        public decimal GetNumberRequestInMonth(int employeeId, RequestType requestType)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmpId = employeeId,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = (int)requestType
            };

            NumberRequestInMonth numberRequest = GetDataEntity<NumberRequestInMonth>(criteria);

            return numberRequest?.NumberRequest ?? 0;
        }

        public LeaveBalanceInMonth GetAnnualLeaveBalanceInMonth(GetLeaveBalanceInMonthCriteria criteria)
        {
            var entityList = GetDataEntity<LeaveBalanceInMonth>(criteria);
            return entityList;
        }

        public IList<DateTime> GetDisableRequestDate(int employeeId)
        {
            var criteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = employeeId,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate
            };

            IList<DateTime> dates = GetData<DateTime>(criteria);
            return dates;
        }

        public DateRange GetDateRange(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            DateRange dateRange = GetDataEntity<DateRange>(criteria);
            return dateRange ?? new DateRange();
        }

        public void SendMailForLeave(LeaveRequestViewModel model)
        {
            if (model != null)
            {
                var criteria = new GetLeadersOfEmployeeCriteria { EmployeeId = model.EmployeeId };
                IList<Employee> leaders = GetData<Employee>(criteria);

                string forDate = GetDateStringForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);
                if (model.IsUnpaidLeave || model.IsAnnualLeaveOver5Day)
                {
                    IList<Employee> managers = GetData<Employee>(new GetManagementsCriteria()).ToList();

                    SendMailToManagers(managers, leaders, model, forDate);
                }
                else
                {
                    SendMailToLeaders(leaders, model, forDate);
                }
            }
        }

        private void SendMailToManagers(
            IList<Employee> managers,
            IList<Employee> leaders,
            LeaveRequestViewModel model,
            string forDate)
        {
            if (managers.Any())
            {
                var body = string.Format(
                    model.EmailTemplate,
                    model.Employee,
                    forDate,
                    model.LeaveTypeName,
                    model.Remark);

                _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    string.Join(";", managers.Select(t => t.Email)),
                    string.Join(";", leaders.Where(t => !t.Email.Equals(model.CurrentUserEmail)).Select(t => t.Email)),
                    model.SubjectEmail,
                    body);
            }
        }

        private void SendMailToLeaders(IList<Employee> leaders, LeaveRequestViewModel model, string forDate)
        {
            if (leaders != null && leaders.Any())
            {
                string body = string.Format(
                    model.EmailTemplate,
                    leaders.First().NickName,
                    model.Employee,
                    forDate,
                    model.LeaveTypeName,
                    model.Remark);

                _emailService.SendEmail(
                    ConfigurationManager.AppSettings["Email"],
                    string.Join(";", leaders.Select(t => t.Email)),
                    string.Empty,
                    model.SubjectEmail,
                    body);
            }
        }

        public void SendMailForCheckInOut(CheckInOutRequestViewModel model)
        {
            if (model != null)
            {
                IList<Employee> leaders = GetLeaderForCheckInOut(model);
                if (leaders != null && leaders.Any())
                {
                    var content = new
                    {
                        Leader = leaders.First().NickName,
                        Member = model.Employee,
                        Type = model.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture),
                        RequestId = model.Id,
                        ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                        Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                        Shift = model.Shift
                    };

                    var body = string.Format(
                        model.EmailTemplate,
                        content.Leader,
                        content.Member,
                        content.Type,
                        content.ForDate,
                        content.Shift,
                        content.Time);

                    _emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        string.Join(";", leaders.Select(t => t.Email)),
                        string.Empty,
                        model.SubjectEmail,
                        body);
                }
            }
        }

        private IList<Employee> GetLeaderForCheckInOut(CheckInOutRequestViewModel model)
        {
            IList<Employee> leaders;

            if (model.IsStandbyAtWeekend)
            {
                var getLeaderCriteria = new GetLeaderForStanbyTeamCriteria();
                leaders = GetData<Employee>(getLeaderCriteria);
            }
            else
            {
                var criteria = new GetLeadersOfEmployeeCriteria
                {
                    EmployeeId = model.EmployeeId
                };

                leaders = GetData<Employee>(criteria);
            }

            return leaders;
        }

        private static string GetDateStringForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            var forDate = totalDays > 1
                ? string.Format(
                    Constants.MailContentLeave,
                    fromDate.ToDateMonthYearFormatedDate(),
                    toDate.ToDateMonthYearFormatedDate(),
                    totalDays)
                : (totalDays == 1
                    ? string.Format(Constants.MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate())
                    : string.Format(
                        Constants.MailContentLeaveHalfDay,
                        fromDate.ToDateMonthYearFormatedDate(),
                        fromDate.ToString("HH:mm"),
                        toDate.ToString("HH:mm")));

            return forDate;
        }

        private IEnumerable<Employee> AddProjectManagerIntoList(IEnumerable<Employee> employees, int employeeId)
        {
            IEnumerable<Employee> results = employees;

            var getEmployeeByTeamsCriteria = new GetEmployeeByTeamsCriteria
            {
                EmployeeId = employeeId,
                TeamCodes = AppSetting.ProjectManagerTeams,
                IgnoreTeams = AppSetting.IgnoreTeams
            };

            var projectManagers = GetData<Employee>(getEmployeeByTeamsCriteria);

            if (results == null)
            {
                return null;
            }

            if (projectManagers != null)
            {
                projectManagers = projectManagers.Where(t => results.All(x => x.Id != t.Id)).ToList();
                results = results.Union(projectManagers);
            }
            else
            {
                results = null;
            }

            return results;
        }

        private RequestEntity GetRequestEntity(long requestId)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                Id = requestId
            };
            var request = _requestRepository.GetRequestEntity(getRequestByIdCriteria);
            return request;
        }

        private void SendMailForProcessRequest(
            long id,
            string command,
            string templateEmailToRequesterPath,
            string templateLeaveEmailToRelatedPath,
            string templateInOutEmailToRelatedPath)
        {
            var request = GetRequestEntity(id);

            SendMailToRequester(request, command, templateEmailToRequesterPath);

            if (command == Command.Approve.EnumToString())
            {
                SendMailToRelatedPeople(request, templateLeaveEmailToRelatedPath, templateInOutEmailToRelatedPath);
            }
        }

        private void SendMailToRequester(RequestEntity request, string command, string templateEmailToRequesterPath)
        {
            var forDate = request.TotalDays > 1
                ? " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : " on " + request.FromDate.ToDateMonthYearFormatedDate();

            var body = Utilities.ReadTextFile(templateEmailToRequesterPath);
            var action = command == Command.Reject.EnumToString()
                ? Labels.Rejected
                : (command == Command.Approve.EnumToString() ? Labels.Approved : Labels.Disapproved);
            body = string.Format(
                body,
                request.Requester.NickName,
                request.RequestType.Name,
                forDate,
                action,
                request.ModifiedBy.NickName,
                request.ModifiedBy.FullName);

            var subject = string.Format(
                Messages.ProcessSubject,
                request.RequestType.Name,
                forDate,
                action,
                request.ModifiedBy.NickName);

            _emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.Requester.Email,
                string.Empty,
                subject,
                body);
        }

        private void SendMailToRelatedPeople(
            RequestEntity request,
            string templateLeaveEmailToRelatedPath,
            string templateInOutEmailToRelatedPath)
        {
            var criteria = new GetEmployeeForRelatedTeamCriteria { EmpId = request.Requester.Id };
            var employees = GetData<Employee>(criteria)
                .Where(i => i.Id != request.ModifiedBy.Id && i.Id != request.Requester.Id && !i.Email.Equals(request.ModifiedBy.Email));

            if (request.RequestType.Id == (int)RequestType.Leave)
            {
                employees = AddProjectManagerIntoList(employees, request.Requester.Id);
            }

            var sendCc = string.Join(",", employees.Select(t => t.Email));
            var sendTo = string.Join(";", employees.Select(t => t.Email));

            switch (request.RequestType.Id)
            {
                case 1:
                case 2:
                    CheckInOutRelatedMail(request, sendCc, templateInOutEmailToRelatedPath);
                    break;

                case 6:
                    LeaveRelatedMail(request, sendTo, templateLeaveEmailToRelatedPath);
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
                request.Requester.DisplayName,
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name,
                request.FromDate.ToString("HH:mm"));

            _emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.ModifiedBy.Email,
                sendCc,
                subject,
                body);
        }

        private void LeaveRelatedMail(RequestEntity request, string sendTo, string templateLeaveEmailToRelatedPath)
        {
            var subject = string.Format(
                Messages.ApprovalNotificationLeaveSubject,
                request.Requester.NickName,
                request.ModifiedBy.NickName);
            var body = Utilities.ReadTextFile(templateLeaveEmailToRelatedPath);
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);

            body = string.Format(
                body,
                $"{request.Requester.NickName} ({request.Requester.FullName})",
                Labels.Leave,
                forDate,
                request.LeaveType.Name,
                request.Remark);

            if (!string.IsNullOrEmpty(sendTo))
            {
                _emailService.SendEmail(
                     ConfigurationManager.AppSettings["Email"],
                     sendTo,
                     string.Empty,
                     subject,
                     body);
            }
        }
    }
}