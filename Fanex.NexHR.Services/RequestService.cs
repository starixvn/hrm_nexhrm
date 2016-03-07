namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
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
    using Fanex.NexHR.ViewModels.CheckInOutRequest;

    public class RequestService : BaseService, IRequestService
    {
        private readonly IRequestRepository requestRepository;
        private readonly IEmailService emailService;
        private readonly ICommandService commandService;

        public RequestService()
            : this(null, null, null)
        {
        }

        public RequestService(
            IRequestRepository requestRepository,
            IEmailService emailService,
            ICommandService commandService)
        {
            this.requestRepository = requestRepository ?? new RequestRepository();
            this.emailService = emailService ?? new EmailService();
            this.commandService = commandService ?? new CommandService();
        }

        public static string ConvertToRequestMessage(RequestEntity request)
        {
            StringBuilder msg = new StringBuilder();
            if (request != null)
            {
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
            }

            return msg.ToString();
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

            CommandResult result = commandService.Update(processRequestCriteria);

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
                    SendMailForProcessRequest(id, command, templateEmailToRequester, templateLeaveEmailToRelated, templateInOutEmailToRelated); 
                });
            }

            return result;
        }

        public IList<RequestEntity> GetRequest(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria != null && criteria.IsValid())
            {
                entityList = requestRepository.GetRequest(criteria);
            }

            return entityList ?? new List<RequestEntity>();
        }

        public RequestEntity GetRequestEntity(GetRequestByIdCriteria criteria)
        {
            RequestEntity request = new RequestEntity();
            if (criteria != null && criteria.IsValid())
            {
                request = requestRepository.GetRequestEntity(criteria);
            }

            return request;
        }

        public IList<RequestEntity> GetRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<RequestEntity> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = requestRepository.GetRequest(criteria);
            }

            return requests ?? new List<RequestEntity>();
        }

        public decimal GetNumberRequestInMonth(int employeeId, RequestType requestType)
        {
            int year = DateTime.Now.Year,
            month = DateTime.Now.Month;
            var criteria = new GetNumberRequestInMonthCriteria()
            {
                EmpId = employeeId,
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = (int)requestType
            };
            NumberRequestInMonth numberRequest = GetDataEntity<NumberRequestInMonth>(criteria);

            return numberRequest == null ? 0 : numberRequest.NumberRequest;
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
                var body = string.Empty;
                var criteria = new GetLeadersOfEmployeeCriteria() { EmployeeId = model.EmployeeId };
                IList<Employee> leaders = GetData<Employee>(criteria);

                string forDate = GetDateStringForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);
                if (model.IsUnpaidLeave || model.IsAnnualLeaveOver5Day)
                {
                    var getManagementsCriteria = new GetManagementsCriteria();
                    IList<Employee> managers = GetData<Employee>(getManagementsCriteria).ToList();

                    if (managers != null && managers.Any())
                    {
                        body = string.Format(model.EmailTemplate, model.Employee, forDate, model.LeaveTypeName, model.Remark);
                        emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            string.Join(";", managers.Select(t => t.Email)),
                            string.Join(";", leaders.Select(t => t.Email)),
                            model.SubjectEmail,
                            body);
                    }
                }
                else
                {
                    if (leaders != null && leaders.Any())
                    {
                        body = string.Format(model.EmailTemplate, leaders.First().NickName, model.Employee, forDate, model.LeaveTypeName, model.Remark);
                        emailService.SendEmail(
                            ConfigurationManager.AppSettings["Email"],
                            string.Join(";", leaders.Select(t => t.Email)),
                            string.Empty,
                            model.SubjectEmail,
                            body);
                    }
                }
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
                            ? RequestType.CheckIn.EnumToString().ToLower()
                            : RequestType.CheckOut.EnumToString().ToLower(),
                        RequestId = model.Id,
                        ForDate = model.ForDate.ToDateMonthYearFormatedDate(),
                        Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                        Shift = model.Shift
                    };

                    var body = string.Format(model.EmailTemplate, content.Leader, content.Member, content.Type, content.ForDate, content.Shift, content.Time);
                    this.emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        string.Join(";", leaders.Select(t => t.Email)),
                        string.Empty,
                        model.SubjectEmail,
                        body);
                }
            }
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

        private IList<Employee> GetLeaderForCheckInOut(CheckInOutRequestViewModel model)
        {
            IList<Employee> leaders = null;
            if (model.IsStandbyAtWeeken)
            {
                var getLeaderCriteria = new GetLeaderForStanbyTeamCriteria();
                leaders = GetData<Employee>(getLeaderCriteria);
            }
            else
            {
                var criteria = new GetLeadersOfEmployeeCriteria()
                {
                    EmployeeId = model.EmployeeId
                };
                leaders = GetData<Employee>(criteria);
            }

            return leaders;
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
            var projectManagers = GetData<Employee>(getEmployeeByTeamsCriteria);

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
            var request = this.requestRepository.GetRequestEntity(getRequestByIdCriteria);

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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.Requester.Email,
                string.Empty,
                subject,
                body);
        }

        private void SendMailToRelatedPeople(RequestEntity request, string templateLeaveEmailToRelatedPath, string templateInOutEmailToRelatedPath)
        {
            var criteria = new GetEmployeeForRelatedTeamCriteria() { EmpId = request.Requester.Id };
            var employees = GetData<Employee>(criteria).Where(i => i.Id != request.ModifiedBy.Id && i.Id != request.Requester.Id);

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
                request.Requester.DisplayName,
                request.RequestType.Name,
                request.FromDate.ToDateMonthYearFormatedDate(),
                request.FromShift.Name,
                request.FromDate.ToString("HH:mm"));

            this.emailService.SendEmail(
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

            this.emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                request.ModifiedBy.Email,
                sendCc,
                subject,
                body);
        }
    }
}