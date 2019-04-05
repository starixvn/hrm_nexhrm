namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Core;
    using Core.Enums;
    using Criteria.SkypeAccount;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Interfaces;
    using Models.NexBot;
    using Models.Request;
    using Models.SkypeAccount;
    using Repositories;
    using Repositories.Interfaces;
    using RestSharp;
    using ViewModels;
    using ViewModels.CheckInOutRequest;

    public class NexbotService : BaseService, INexbotService
    {
        private readonly IRestClient webClient;
        private readonly INexbotRepository nexbotRepository;

        public NexbotService()
        {
            webClient = new RestClient { Encoding = Encoding.UTF8 };
            nexbotRepository = new NexbotRepository();
        }

        public bool SendMessage(string conversationId, string message)
        {
            if (!string.IsNullOrWhiteSpace(conversationId))
            {
                return ForwardToBot(message, conversationId).IsSuccessful;
            }

            return false;
        }

        private RestRequest CreateRequestHeader(string resource, Method method)
        {
            var token = GetToken();

            var request = new RestRequest(resource, method);
            request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");

            return request;
        }

        public IList<ConversationUser> GetSkypeAccount()
        {
            var request = CreateRequestHeader("/account/index", Method.GET);

            webClient.BaseUrl = NexBotConfiguration.BotServiceUrl;
            IRestResponse<List<ConversationUser>> response = webClient.Execute<List<ConversationUser>>(request);
            var skypeAccounts = response.Data;

            return skypeAccounts;
        }

        public void SendApproveMessage(string conversationId, long requestId, string message)
        {
            var request = CreateRequestHeader("/request/approve", Method.POST);

            request.AddParameter("conversationId", conversationId);
            request.AddParameter("requestId", requestId);
            request.AddParameter("message", message);

            webClient.BaseUrl = NexBotConfiguration.BotServiceUrl;
            webClient.Execute(request);
        }

        private IRestResponse ForwardToBot(string message, string conversationId)
        {
            var request = CreateRequestHeader("/messages/forward", Method.POST);
            request.AddParameter("message", message);
            request.AddParameter("conversationId", conversationId);

            webClient.BaseUrl = NexBotConfiguration.BotServiceUrl;
            return webClient.Execute(request);
        }

        private Token GetToken()
        {
            webClient.BaseUrl = NexBotConfiguration.TokenUrl;

            var request = new RestRequest(Method.POST);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", NexBotConfiguration.ClientId);
            request.AddParameter("client_secret", NexBotConfiguration.ClientPassword);
            request.AddParameter("scope", $"{NexBotConfiguration.ClientId}/.default");

            request.AddHeader("Content-type", "application/x-www-form-urlencoded");

            var response = webClient.Execute<Token>(request);

            return response.Data;
        }

        public IEnumerable<EmployeeSkypeAccount> GetEmployeeSkypeAccount(GetEmployeeSkypeAccountCriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                var employeeAccounts = nexbotRepository.GetEmployeeSkypeAccount(criteria).ToList();

                var conversationUsers = GetSkypeAccount();

                return CombineEmployeeSkypeAccount(employeeAccounts, conversationUsers);
            }

            return new List<EmployeeSkypeAccount>();
        }

        private static IList<EmployeeSkypeAccount> CombineEmployeeSkypeAccount(IList<EmployeeSkypeAccount> employeeAccounts, IList<ConversationUser> conversationUsers)
        {
            foreach (var account in employeeAccounts)
            {
                var conversation = conversationUsers.FirstOrDefault(t => t.ConversationId == account.ConversationId);
                if (conversation != null)
                {
                    account.SkypeDisplayName = conversation.UserName;
                }
            }

            return employeeAccounts;
        }

        public void SendMessageForLeaveRequest(LeaveRequestViewModel request)
        {
            if (request != null)
            {
                IEnumerable<Employee> leaders;
                if (request.IsAnnualLeaveOver5Day || request.IsUnpaidLeave)
                {
                    leaders = GetData<Employee>(new GetManagementsCriteria());
                }
                else
                {
                    var criteria = new GetLeadersOfEmployeeCriteria
                    {
                        EmployeeId = request.EmployeeId
                    };
                    leaders = GetData<Employee>(criteria);
                }
                SendMessageToHandleLeaveRequestMembers(leaders, request);
            }
        }

        private void SendMessageToHandleLeaveRequestMembers(IEnumerable<Employee> leaders, LeaveRequestViewModel request)
        {
            if (leaders != null && leaders.Any())
            {
                foreach (var leader in leaders)
                {
                    var message = GenerateMessageForLeaveRequest(request, leader.NickName);
                    SendApproveMessage(leader.ConversationId, request.Id, message);
                }
            }
        }

        private static string GenerateMessageForLeaveRequest(LeaveRequestViewModel request, string leaderName)
        {
            var forDate = GetDateStringForLeaveRequest(request.FromDate, request.ToDate, request.TotalDays);
            var message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is inform you about your member, {request.Employee} had a leave request {Constants.NewLine}";
            message += $"_- Date: {forDate}_ {Constants.NewLine}";
            message += $"_- Type: {request.LeaveTypeName}_ {Constants.NewLine}";
            message += $"_- Remark: {request.Remark}_ {Constants.NewLine}";
            message += $"Please process this request at: http://hrm.nexcel.vn/request/member.  {Constants.NewLine}";
            message += "Any questions, please contact HR department.";
            return message;
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

        public void SendMessageProcessRequest(RequestEntity request, string command)
        {
            var employeee = GetDataEntity<Employee>(new GetEmployeeSkypeAccountCriteria { EmployeeId = request.Requester.Id });
            if (employeee != null)
            {
                var message = GenerateProcessRequestMessage(request, command);
                SendMessage(employeee.ConversationId, message);
            }
        }

        private static string GenerateProcessRequestMessage(RequestEntity request, string command)
        {
            var action = command == Command.Reject.EnumToString() ? $"{command.ToLowerInvariant()}ed" : $"{command.ToLowerInvariant()}d";

            var forDate = request.TotalDays > 1
                ? "from " + request.FromDate.ToDateMonthYearFormatedDate() + " to " +
                  request.ToDate.ToDateMonthYearFormatedDate()
                : "on " + request.FromDate.ToDateMonthYearFormatedDate();
            var message = $"Dear {request.Requester.NickName},{Constants.NewLine}";
            message += $"Your {request.RequestType.Name.ToLowerInvariant()} request _{forDate}_ has been {action} by _{request.ModifiedBy.DisplayName}._{Constants.NewLine}";
            message += $"For more detail, please visit: https://hrm.nexcel.vn. Thanks";
            return message;
        }

        public void SendMessageForCheckInOutRequest(CheckInOutRequestViewModel request, IList<Employee> leaders)
        {
            foreach (var leader in leaders)
            {
                var message = GenerateCheckInOutRequestMessage(request, leader.NickName);
                SendApproveMessage(leader.ConversationId, request.Id, message);
            }
        }

        private static string GenerateCheckInOutRequestMessage(CheckInOutRequestViewModel request, string leaderName)
        {
            string type = request.CheckInOutType == (short)RequestType.CheckIn
                            ? RequestType.CheckIn.EnumToString().ToLower(CultureInfo.InvariantCulture)
                            : RequestType.CheckOut.EnumToString().ToLower(CultureInfo.InvariantCulture);
            string time = request.Hour.ToString().PadLeft(2, '0') + ":" + request.Minute.ToString().PadLeft(2, '0');
            string message = $"Dear {leaderName}, {Constants.NewLine}";
            message += $"This is to inform you about your member, {request.Employee} had a {type} request:{Constants.NewLine}";
            message += $"_- Date: {request.WorkDate.ToDateMonthYearFormatedDate()}_{Constants.NewLine}";
            message += $"_- Shift: {request.Shift}_{Constants.NewLine}";
            message += $"_- Time: {time}_{Constants.NewLine}";
            message += $"Please process this request at: https://hrm.nexcel.vn/request/member. {Constants.NewLine}";
            message += $"Any questions, please contact HR department. Thanks";
            return message;
        }
    }
}