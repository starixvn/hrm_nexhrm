namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Request;
    using Data.Repository;
    using Fanex.NexHR.Models.Core;
    using Fanex.NexHR.Models.LateInEarlyOut;
    using Fanex.NexHR.Models.Staff;
    using Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Models;
    using Models.Request;
    using Repositories.Interfaces;
    using Resources;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class RequestService : IRequestService
    {
        private readonly IRequestRepository requestRepository;
        private readonly IEmailService emailService;
        private readonly INexbotService nexbotService;
        private readonly IWebHostEnvironment environment;
        private readonly IBaseService baseService;
        private readonly AppSetting appSetting;
        private readonly EmailSettings emailSettings;
        private readonly ILogServiceProvider logService;

        private const string EmailTemplatePath = "EmailTemplates/AutoJobDeduct.Request.txt";
        private const string stringTime = "HH:mm";

#pragma warning disable S107 // Methods should not have too many parameters

        public RequestService(
            IRequestRepository requestRepository,
            IEmailService emailService,
            INexbotService nexbotService,
            IWebHostEnvironment environment,
            AppSetting appSetting,
            EmailSettings emailSettings,
            IBaseService baseService,
            ILogServiceProvider logService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.requestRepository = requestRepository;
            this.emailService = emailService;
            this.nexbotService = nexbotService;
            this.environment = environment;
            this.appSetting = appSetting;
            this.emailSettings = emailSettings;
            this.baseService = baseService;
            this.logService = logService;
        }

        private static string GetMessageDateforLeaveRequest(DateTime fromDate, DateTime toDate)
        {
            if ((toDate - fromDate).Days > 0)
            {
                if (fromDate.Year == toDate.Year)
                {
                    if (fromDate.Month != toDate.Month)
                    {
                        return fromDate.ToString("dd MMM") + " - " + toDate.ToDateMonthYearFormatedDate();
                    }

                    return fromDate.ToString("dd") + " - " + toDate.ToDateMonthYearFormatedDate();
                }

                return fromDate.ToDateMonthYearFormatedDate() + " - " + toDate.ToDateMonthYearFormatedDate();
            }

            return fromDate.ToDateMonthYearFormatedDate();
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
                            request.FromDate.ToString(stringTime) + (request.IsCrossDate ? "*" : string.Empty),
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
                    request.FromDate.ToString(stringTime),
                    request.ToDate.ToString(stringTime));
            }

            return msg;
        }

        public async Task<CommandResult> Process(string command, long id, int processBy)
        {
            INonQueryCommand processRequestCriteria = GetProcessRequestCriteria(command, id, processBy);

            CommandResult result = await baseService.ExecuteNonQueryAsync(processRequestCriteria);
            if (result.IsSuccessful)
            {
                string templateEmailToRequester = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForRequester);
                string templateLeaveEmailToRelated = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForLeaveApproved);
                string templateLeaveEmailToHR = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForLeaveDisApproved);
                string templateInOutEmailToRelated = Path.Combine(environment.ContentRootPath, Constants.EmailTemplatePathForCheckInOutApproved);

                RunBackgroundJobs(
                    new[] {
                        SendMailForProcessRequest(
                                id,
                                command,
                                templateEmailToRequester,
                                templateLeaveEmailToRelated,
                                templateInOutEmailToRelated,
                                templateLeaveEmailToHR),

                        nexbotService.SendMessageProcessRequest(await GetRequestEntity(id), command)
            });
            }

            return result;
        }

        private static INonQueryCommand GetProcessRequestCriteria(string command, long id, int processBy)
        {
            INonQueryCommand processRequestCriteria;
            switch (command)
            {
                case "Disapprove":
                    processRequestCriteria = new DisapproveRequestCriteria
                    {
                        RequestId = id,
                        ModifiedBy = processBy,
                        ModifiedDate = DateTimeOffset.Now.DateTime
                    };
                    break;

                case "Reject":
                    processRequestCriteria = new RejectRequestCriteria
                    {
                        RequestId = id,
                        ProcessedBy = processBy,
                        ProcessedDate = DateTimeOffset.Now.DateTime
                    };
                    break;

                default:
                    processRequestCriteria = new ApproveRequestCriteria
                    {
                        RequestId = id,
                        ApprovedBy = processBy,
                        ApprovedDate = DateTimeOffset.Now.DateTime
                    };
                    break;
            }

            return processRequestCriteria;
        }

        public async Task<IList<RequestEntity>> GetRequest(GetRequestCriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();

            if (criteria != null && criteria.IsValid())
            {
                entityList = await requestRepository.GetRequest(criteria);
            }

            return entityList ?? new List<RequestEntity>();
        }

        public async Task<RequestEntity> GetRequestEntity(GetRequestByIdCriteria criteria)
        {
            RequestEntity request = new RequestEntity();
            if (criteria != null && criteria.IsValid())
            {
                request = await requestRepository.GetRequestEntity(criteria);
            }

            return request;
        }

        public async Task<IList<RequestEntity>> GetRequestHistory(GetRequestHistoryCriteria criteria)
        {
            IList<RequestEntity> requests = null;
            if (criteria != null && criteria.IsValid())
            {
                requests = await requestRepository.GetRequest(criteria);
            }

            return requests ?? new List<RequestEntity>();
        }

        public async Task<decimal> GetNumberRequestInMonth(int employeeId, RequestType requestType)
        {
            int year = DateTimeOffset.Now.Year;
            int month = DateTimeOffset.Now.Month;

            var criteria = new GetNumberRequestInMonthCriteria
            {
                EmpId = employeeId,
                FromDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, TimeSpan.Zero).DateTime,
                RequestType = (int)requestType
            };

            NumberRequestInMonth numberRequest = await baseService.GetDataEntityAsync<NumberRequestInMonth>(criteria);

            return numberRequest?.NumberRequest ?? 0;
        }

        public async Task<LeaveBalanceInMonth> GetAnnualLeaveBalanceInMonth(GetLeaveBalanceInMonthCriteria criteria)
        {
            var entityList = await baseService.GetDataEntityAsync<LeaveBalanceInMonth>(criteria);
            return entityList;
        }

        public async Task<IEnumerable<DateTime>> GetDisableRequestDate(int employeeId, long? requestId)
        {
            var criteria = new GetUnavailableRequestDateCriteria
            {
                EmployeeId = employeeId,
                FromDate = Utilities.RequestAvailableFromDate,
                ToDate = Utilities.RequestAvailableToDate,
                RequestId = requestId
            };

            IEnumerable<DateTime> dates = await baseService.GetDataAsync<DateTime>(criteria);
            return dates;
        }

        public async Task<DateRange> GetDateRange(GetDateRangeEmployeeCalendarCriteria criteria)
        {
            DateRange dateRange = await baseService.GetDataEntityAsync<DateRange>(criteria);
            return dateRange ?? new DateRange();
        }

        public void SendMailForLeave(LeaveRequestViewModel model, IEnumerable<EmployeeSupervisor> employeeSupervisors)
        {
            string forDate = GetDateStringForLeaveRequest(model.FromDate, model.ToDate, model.TotalDays);

            if (employeeSupervisors != null && employeeSupervisors.Any())
            {
                string body = string.Format(
                    model.EmailTemplate,
                    employeeSupervisors.FirstOrDefault()?.SupervisorNickName,
                    model.Employee,
                    forDate,
                    model.LeaveTypeName,
                    model.Remark,
                    appSetting.BaseUrl,
                    emailSettings.CompanyMailSign);

                emailService.SendEmail(
                    emailSettings.Email,
                    string.Join(";", employeeSupervisors.Where(t => !t.IsReadOnly).Select(t => t.SupervisorEmail)),
                    string.Join(";", employeeSupervisors.Where(t => t.IsReadOnly).Select(t => t.SupervisorEmail)),
                    model.SubjectEmail,
                    body);
            }
        }

        public void SendMailDeductLateInEarlyOutToEmp(LateInEarlyOutModel lateInEarlyOutModel)
        {
            string templatePath = Path.Combine(environment.ContentRootPath, EmailTemplatePath);
            string body = string.Format(
                   Utilities.ReadTextFile(templatePath),
                   lateInEarlyOutModel.NickName,
                   emailSettings.CompanyMailSign);

            emailService.SendEmail(
                emailSettings.Email,
                string.Join(";", lateInEarlyOutModel.Email),
                string.Empty,
                Messages.SubjectEmailAutoDeduct,
                body);
        }

        public void SendMailForCheckInOut(CheckInOutRequestViewModel model, IEnumerable<EmployeeSupervisor> employeeSupervisors)
        {
            try
            {
                var content = new
                {
                    Leader = employeeSupervisors.FirstOrDefault()?.SupervisorNickName,
                    Member = model.Employee,
                    Type = model.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture),
                    RequestId = model.Id,
                    ForDate = model.WorkDate.ToDateMonthYearFormatedDate(),
                    Time = model.Hour.ToString().PadLeft(2, '0') + ":" + model.Minute.ToString().PadLeft(2, '0'),
                    model.Shift
                };

                var body = string.Format(
                    model.EmailTemplate,
                    content.Leader,
                    content.Member,
                    content.Type,
                    content.ForDate,
                    content.Shift,
                    content.Time,
                    appSetting.BaseUrl,
                    emailSettings.CompanyMailSign);

                emailService.SendEmail(
                    emailSettings.Email,
                    string.Join(";", employeeSupervisors.Select(t => t.SupervisorEmail)),
                    string.Empty,
                    model.SubjectEmail,
                    body);
            }
            catch (Exception ex)
            {
                logService.LogException(ex);
            }
        }

        public async Task<IEnumerable<EmployeeSupervisor>> GetLeaderForCheckInOut(int employeeId, bool isStandbyAtWeekend)
        {
            var criteria = new GetSupervisorForCheckInOutRequestCriteria
            {
                EmployeeId = employeeId,
                IsStandbyAtWeekend = isStandbyAtWeekend
            };

            var leaders = await baseService.GetDataAsync<EmployeeSupervisor>(criteria);
            return leaders;
        }

        public async Task<IEnumerable<EmployeeSupervisor>> GetLeaderForLeave(int employeeId, bool isManagementApprove)
        {
            var criteria = new GetSupervisorForLeaveRequestCriteria
            {
                EmployeeId = employeeId,
                IsManagementApprove = isManagementApprove
            };

            var leaders = await baseService.GetDataAsync<EmployeeSupervisor>(criteria);
            return leaders;
        }

        private static string GetDateStringForLeaveRequest(DateTime fromDate, DateTime toDate, decimal totalDays)
        {
            if (totalDays > 1)
            {
                return string.Format(
                    Constants.MailContentLeave,
                    fromDate.ToDateMonthYearFormatedDate(),
                    toDate.ToDateMonthYearFormatedDate(),
                    totalDays);
            }
            else
            {
                if (totalDays == 1)
                {
                    return string.Format(Constants.MailContentLeaveOneDay, fromDate.ToDateMonthYearFormatedDate());
                }

                return string.Format(
                        Constants.MailContentLeaveHalfDay,
                        fromDate.ToDateMonthYearFormatedDate(),
                        fromDate.ToString(stringTime),
                        toDate.ToString(stringTime));
            }
        }

        private async Task<IEnumerable<Employee>> AddProjectManagerIntoList(IEnumerable<Employee> employees, int employeeId)
        {
            IEnumerable<Employee> results = employees;

            var getEmployeeByTeamsCriteria = new GetEmployeeByTeamsCriteria
            {
                EmployeeId = employeeId,
                TeamCodes = appSetting.ProjectManagerTeams,
                IgnoreTeams = appSetting.IgnoreTeams
            };

            var projectManagers = await baseService.GetDataAsync<Employee>(getEmployeeByTeamsCriteria);

            if (results == null)
            {
                return results;
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

        private async Task<RequestEntity> GetRequestEntity(long requestId)
        {
            var getRequestByIdCriteria = new GetRequestByIdCriteria
            {
                Id = requestId
            };
            var request = await requestRepository.GetRequestEntity(getRequestByIdCriteria);
            return request;
        }

        private async Task SendMailForProcessRequest(
            long id,
            string command,
            string templateEmailToRequesterPath,
            string templateLeaveEmailToRelatedPath,
            string templateInOutEmailToRelatedPath,
            string templateLeaveEmailToHR)
        {
            var request = await GetRequestEntity(id);

            SendMailToRequester(request, command, templateEmailToRequesterPath);

            if (command == Command.Approve.EnumToString())
            {
                await SendMailToRelatedPeople(request, templateLeaveEmailToRelatedPath, templateInOutEmailToRelatedPath);
            }

            if (command == Command.Disapprove.EnumToString())
            {
                await SendMailForHR(request, templateLeaveEmailToHR);
            }
        }

        private void SendMailToRequester(RequestEntity request, string command, string templateEmailToRequesterPath)
        {
            var forDate = request.TotalDays > 1
                ? " from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : " on " + request.FromDate.ToDateMonthYearFormatedDate();

            var body = Utilities.ReadTextFile(templateEmailToRequesterPath);
            var action = Labels.Disapproved;

            if (command == Command.Reject.EnumToString())
            {
                action = Labels.Rejected;
            }
            else
            {
                if (command == Command.Approve.EnumToString())
                {
                    action = Labels.Approved;
                }
            }

            body = string.Format(
                body,
                request.Requester.NickName,
                request.RequestType.Name,
                forDate,
                action,
                request.ModifiedBy.NickName,
                request.ModifiedBy.FullName,
                appSetting.BaseUrl,
                emailSettings.CompanyMailSign);

            var subject = string.Format(
                Messages.ProcessSubject,
                request.RequestType.Name,
                forDate,
                action,
                request.ModifiedBy.NickName);

            emailService.SendEmail(
                emailSettings.Email,
                request.Requester.Email,
                string.Empty,
                subject,
                body);
        }

        private async Task SendMailToRelatedPeople(
            RequestEntity request,
            string templateLeaveEmailToRelatedPath,
            string templateInOutEmailToRelatedPath)
        {
            var criteria = new GetEmployeeForRelatedTeamCriteria { EmpId = request.Requester.Id };
            var dataEmps = await baseService.GetDataAsync<Employee>(criteria);
            var employees = dataEmps.Where(i => i.Id != request.ModifiedBy.Id && i.Id != request.Requester.Id && i.Email != null && !i.Email.Equals(request.ModifiedBy.Email));

            if (request.RequestType.Id == (int)RequestType.Leave)
            {
                employees = await AddProjectManagerIntoList(employees, request.Requester.Id);
            }

            var sendCc = string.Join(",", employees.Select(t => t.Email));
            var sendTo = string.Join(";", employees.Select(t => t.Email));

            if (request.RequestType.Id == (int)RequestType.Leave)
            {
                LeaveRelatedMail(request, sendTo, templateLeaveEmailToRelatedPath);
            }
            else
            {
                CheckInOutRelatedMail(request, sendCc, templateInOutEmailToRelatedPath);
            }
        }

        private async Task SendMailForHR(RequestEntity request,
            string templateLeaveEmailToHR)
        {
            IEnumerable<Employee> HRs = await baseService.GetDataAsync<Employee>(new GetHRCriteria());

            var sendTo = string.Join(";", HRs.Select(t => t.Email));
            var subject = string.Format(
                Messages.DisApprovalNotificationLeaveSubject,
                request.Requester.NickName,
                request.ModifiedBy.NickName);
            var body = Utilities.ReadTextFile(templateLeaveEmailToHR);
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);

            body = string.Format(
                body,
                $"{request.Requester.NickName} ({request.Requester.FullName})",
                Labels.Leave,
                forDate,
                request.LeaveType.Name,
                request.Remark,
                emailSettings.CompanyMailSign);

            if (!string.IsNullOrEmpty(sendTo))
            {
                emailService.SendEmail(
                     emailSettings.Email,
                     sendTo,
                     string.Empty,
                     subject,
                     body);
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
                request.FromDate.ToString(stringTime),
                emailSettings.CompanyMailSign);

            emailService.SendEmail(
                emailSettings.Email,
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
                request.Remark,
                emailSettings.CompanyMailSign);

            if (!string.IsNullOrEmpty(sendTo))
            {
                emailService.SendEmail(
                     emailSettings.Email,
                     sendTo,
                     string.Empty,
                     subject,
                     body);
            }
        }

        private static void RunBackgroundJobs(Task[] tasks)
        {
            Task.Run(() => tasks);
        }

        public async Task<IEnumerable<ApproverRequestSumup>> GetPendingRequestGroupByApproverAsync()
        {
            var pendingRequests = await baseService.GetDataAsync<PendingRequestApprover>(new GetPendingRequestCriteria());
            var needApprovedRequests = pendingRequests.GroupBy(r => r.ApproverId)
                .Select(g => new ApproverRequestSumup
                {
                    ApproverId = g.Key,
                    ApproverNickname = g.FirstOrDefault().Approver,
                    ConversationId = g.FirstOrDefault().ApproverConversationId,
                    CallbackUrl = appSetting.BaseUrl,
                    Requests = g.Select(i => new MemberRequest
                    {
                        RequestId = i.RequestId,
                        NickName = i.Requester,
                        TeamName = i.TeamName,
                        FullName = i.FullNameRequester,
                        ApproverId = i.ApproverId,
                        Avatar = string.Format("{0}/Avatars/{1}.jpg", appSetting.BaseUrl, i.Code),
                        IsLeaveRequest = !IsCheckInOutRequest(i.RequestType),
                        DateDetails = GenerateDisplayDate(i.FromDate, i.ToDate, i.RequestType, i.TotalDays, i.FromShift),
                        FromDate = i.FromDate.ToDateMonthYearFormatedDate(),
                        TotalDays = i.TotalDays > 1 ? $"({i.TotalDays} days)" : string.Empty,
                        RequestType = IsCheckInOutRequest(i.RequestType) ? i.RequestType.EnumToString().Trim() : DisplayLeaveTypeName(i.LeaveTypeName, i.TotalDays),
                        Remark = IsCheckInOutRequest(i.RequestType) ? "-" : i.Remark,
                        CallbackUrl = appSetting.BaseUrl,
                        CreatedDate = i.CreatedDate.ToFormattedDateTime(),
                        CreatedAt = i.CreatedDate
                    })
                });

            return needApprovedRequests;
        }

        private static string DisplayLeaveTypeName(string leaveTypeName, decimal totalDays) =>
            string.Format("{0}{1}", leaveTypeName, totalDays < 1 ? " (½)" : string.Empty).Trim();

        private static string GenerateDisplayDate(DateTime? from, DateTime? to, RequestType requestType, decimal totalDays, string shiftName)
        {
            string displayDate;
            if (IsCheckInOutRequest(requestType))
            {
                displayDate = $"{from.Value.ToString(stringTime)} {shiftName}";
            }
            else if (totalDays < 1)
            {
                displayDate = string.Format("{0} ({1} > {2})", from.Value.ToDateMonthYearFormatedDate(), from.Value.ToString(stringTime), to.Value.ToString(stringTime));
            }
            else
            {
                displayDate = DisplayLeaveDateDetails(from, to, totalDays);
            }
            return displayDate;
        }

        private static bool IsCheckInOutRequest(RequestType requestType) => requestType == RequestType.CheckIn || requestType == RequestType.CheckOut;

        private static string DisplayLeaveDateDetails(DateTime? from, DateTime? to, decimal totalDays)
        {
            if (totalDays == 1)
            {
                return from.Value.ToDateMonthYearFormatedDate();
            }

            if (from.Value.Year == to.Value.Year && from.Value.Month == to.Value.Month)
            {
                return $"{from.Value.ToString("dd")} > {to.Value.ToDateMonthYearFormatedDate()}";
            }

            if (from.Value.Year == to.Value.Year && from.Value.Month != to.Value.Month)
            {
                return $"{from.Value.ToString("dd, MMM")} > {to.Value.ToDateMonthYearFormatedDate()}";
            }

            return $"{from.Value.ToDateMonthYearFormatedDate()} > {to.Value.ToDateMonthYearFormatedDate()}";
        }
    }
}